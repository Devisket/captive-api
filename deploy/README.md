# Captive backend — Windows Server deployment

Deploys the three APIs to **IIS** and `Captive.Orchestrator` as a **Windows Service**.

No C# was changed. Everything here works against the code as it stands today,
which means a few rough edges are worked around rather than fixed — those are
listed under [Known limitations](#known-limitations), with the code fix for each.

```
deploy/
├── Install-Prerequisites.ps1   one-time server prep
├── Publish-Captive.ps1         dotnet publish → artifact folder
├── Deploy-Captive.ps1          install / update everything
├── config/                     per-server settings  ← EDIT THESE FIRST
│   ├── Commands.appsettings.json
│   ├── Query.appsettings.json
│   ├── MdbApi.appsettings.json
│   └── Orchestrator.appsettings.json
└── winsw/
    └── Captive.Orchestrator.xml   service definition template
```

---

## What gets deployed where

| Project | Hosting | Location |
|---|---|---|
| `Captive.Commands` | IIS app `/commands`, pool `CaptiveCommands` | `C:\Captive\apps\Commands` |
| `Captive.Queries` | IIS app `/query`, pool `CaptiveQuery` | `C:\Captive\apps\Query` |
| `Captive.MdbAPI` | IIS app `/mdbapi`, pool `CaptiveMdbApi` (**32-bit**) | `C:\Captive\apps\MdbApi` |
| `Captive.Orchestrator` | Windows Service `CaptiveOrchestrator` (via WinSW) | `C:\Captive\apps\Orchestrator` |

`Captive.Barcode` is **not** deployed — barcode generation stays as
`BarcodeGenerator.exe`, launched by the Orchestrator.

---

## First run on a new server

Run all of this from an **elevated PowerShell** session.

### 1. Prerequisites

```powershell
cd C:\path\to\captive-api\deploy
.\Install-Prerequisites.ps1
```

Enables the IIS features, verifies the Hosting Bundle, creates `C:\Captive\…`,
and downloads WinSW.

If it warns about the **ASP.NET Core Module** or the **x86 runtime**, install the
[.NET 8 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0) and re-run.
Order matters: **IIS first, then the Hosting Bundle.** If you install the .NET SDK
afterwards, repair the Hosting Bundle or IIS loses the module registration.

Not automated — do these by hand:

- Install SQL Server and RabbitMQ, or point `config/` at existing hosts.
- Install the **32-bit Microsoft Access Database Engine (ACE OLEDB)** — `Captive.MdbAPI` needs it.
- Set up the barcode generator (see [Barcode generator](#barcode-generator) below).

### 2. Edit the config

Every file in `config/` ships with `CHANGEME` placeholders. `Deploy-Captive.ps1`
**refuses to run** while any remain, so nothing can reach a server half-configured.

Set connection strings, the RabbitMQ host, and the `Endpoints` URLs (which must
match the `-HttpPort` you deploy with).

### 3. Publish

```powershell
.\Publish-Captive.ps1
```

Needs the **.NET 8 SDK** on whatever machine runs it. Writes to
`C:\Captive\artifacts\<timestamp>\` and prints the path.

`Captive.MdbAPI` is published `win-x86`; everything else `win-x64`. All four are
framework-dependent — the Hosting Bundle supplies the runtime.

### 4. Deploy

```powershell
.\Deploy-Captive.ps1 -ArtifactPath C:\Captive\artifacts\20260825-143000
```

Stops the pools and service, mirrors the files, applies config, configures IIS,
installs the service, starts everything, and smoke-tests the endpoints.

Defaults to `-Environment Development` (Swagger and detailed errors — what you
want on a test server) and `-HttpPort 8080` (avoids the Default Web Site).

---

## Routine redeploys

```powershell
$artifact = .\Publish-Captive.ps1
.\Deploy-Captive.ps1 -ArtifactPath $artifact
```

`Deploy-Captive.ps1` is idempotent. To move one app only:

```powershell
.\Deploy-Captive.ps1 -ArtifactPath $artifact -Apps Orchestrator
```

---

## Enabling HTTPS

Self-signed certificate for a test server:

```powershell
$cert = New-SelfSignedCertificate -DnsName 'captive-test.local' -CertStoreLocation 'Cert:\LocalMachine\My'
.\Deploy-Captive.ps1 -ArtifactPath $artifact -CertThumbprint $cert.Thumbprint
```

With a real certificate, import it to `LocalMachine\My` and pass its thumbprint.

**Without** a certificate the site is HTTP-only. All three APIs call
`UseHttpsRedirection()`, but with no HTTPS binding the middleware cannot resolve
a target port, logs a warning, and passes the request through — so HTTP works.
You will see `Failed to determine the https port for redirect` in the log. That
is expected on an HTTP-only deployment, not a fault.

---

## Known limitations

Each of these exists because this pass changed no C#.

### CORS is pinned to `localhost:4200`

Both `Captive.Commands` and `Captive.Queries` hardcode:

```csharp
.WithOrigins("http://localhost:4200", "https://localhost:4200")
```

An Angular client served from anything else — including the test server's own
hostname — **will be blocked by the browser**. Scripts cannot fix this; the
origins have to come from config.

*Fix:* read origins from `appsettings.json`.

### The Orchestrator ignores environment variables

`Captive.Orchestrator/Program.cs` does:

```csharp
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json");     // ← appended LAST
```

`Host.CreateApplicationBuilder` has *already* loaded `appsettings.json`,
`appsettings.{Environment}.json`, environment variables and command-line args.
Appending `appsettings.json` again puts it at the **highest** priority, so it
overrides all of them.

Consequences:

- Environment variables silently do nothing for this app.
- An `appsettings.Production.json` overlay silently does nothing.
- `Deploy-Captive.ps1` therefore **replaces** `appsettings.json` outright.

*Fix:* delete both appended lines — the defaults already cover them.

### `Captive.Commands` ships its Development settings

The csproj forces `appsettings.Development.json` into the publish output with
`CopyToPublishDirectory=Always`. It carries development connection strings.
Harmless under `-Environment Production` (it isn't loaded), but it is on disk.
`Publish-Captive.ps1` warns; delete it by hand if that matters.

### Migrations run at startup

`Captive.Commands` calls `MigrateDatabase()` before `app.Run()`. If SQL Server is
unreachable or a migration is slow, the app pool fails to start and the smoke
test reports it. Check Event Viewer → Application, source
*IIS AspNetCore Module V2*.

### No application logging

`Captive.Commands` does `ClearProviders().AddConsole()`; the other two use console
defaults. **Under IIS, console output goes nowhere.** Right now you get startup
failures in the Event Log and nothing else.

Two options until Serilog lands:

1. **Enable ASP.NET Core stdout logging** — edit the deployed `web.config`:
   ```xml
   <aspNetCore ... stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout">
   ```
   Diagnostic only; it is unbuffered and grows without limit. Turn it back off.
   It is also overwritten by the next deploy.
2. **Add Serilog** — the real fix, deliberately out of scope for this pass.

The Orchestrator is better off: WinSW captures its console output to
`C:\Captive\logs\Orchestrator\`, rolling at 50 MB, 10 files kept.

### Why the Orchestrator needs WinSW

`Captive.Orchestrator` is a plain generic-host console app — it never calls
`UseWindowsService()`. Registered directly with `sc.exe` it would start and then
be killed with **error 1053**, because it never responds to the Service Control
Manager.

WinSW (MIT) runs it as a child process, speaks SCM on its behalf, sets the
working directory (needed for that relative `AddJsonFile` call), and redirects
its output to rolling log files.

*Fix:* add `Microsoft.Extensions.Hosting.WindowsServices` and one line —
`builder.Services.AddWindowsService(...)` — then WinSW can go away.

---

## Barcode generator

Not automated, because two of its files are not in source control.

1. Build `MBTC.BarcodeGenerator` (.NET Framework 4.7.2, x86) and copy the output
   to `C:\Captive\apps\BarcodeGenerator\`.
2. Copy these next to `BarcodeGenerator.exe` — `clsBcConfig.set_ConfigPath()`
   reads them from the exe's own folder:
   - `tMasDigits.ini` *(in git)*
   - `acct_mapping.txt` **(not in git)**
   - `serial_mapping.txt` **(not in git)**

   ⚠️ The last two exist only under `MBTC.BarcodeGenerator\bin\Debug\`, which is
   gitignored. **A clean clone will not produce them.** Copy them from a working
   machine, then get them into source control.

3. Register the COM component with the **32-bit** regsvr32:
   ```powershell
   C:\Windows\SysWOW64\regsvr32.exe C:\Captive\apps\BarcodeGenerator\BcConfig.dll
   ```
   `BcConfig.dll` is a COM library (`<COMReference>` in the csproj). This is a
   machine-level step that does not travel with a file copy — the thing most
   easily forgotten when rebuilding a server.

4. Confirm `BarcodeService:Mtbc` in `config/Orchestrator.appsettings.json` points
   at the deployed exe. The repo default still points at a developer's
   `bin\Debug` path.

---

## Troubleshooting

| Symptom | Where to look |
|---|---|
| App pool starts then stops | Event Viewer → Application, source *IIS AspNetCore Module V2* |
| HTTP 500.30 (start failure) | Usually the DB connection or a migration. Enable stdout logging. |
| HTTP 500.19 | Missing `web.config`, or the Hosting Bundle is not installed. |
| HTTP 502.5 | Bitness mismatch — check *Enable 32-Bit Applications* matches the publish runtime. |
| Service won't start | `C:\Captive\logs\Orchestrator\CaptiveOrchestrator.wrapper.log` |
| Orchestrator restart loop | Almost always RabbitMQ unreachable. Check `Rabbitmq:Hostname`. |
| Barcode calls fail | COM not registered (step 3), or a mapping file missing (step 2). |

Useful commands:

```powershell
Get-WebAppPoolState -Name CaptiveCommands
Get-Service CaptiveOrchestrator
Get-Content C:\Captive\logs\Orchestrator\CaptiveOrchestrator.out.log -Tail 50 -Wait
C:\Captive\apps\Orchestrator\CaptiveOrchestrator.exe status
```

---

## Rollback

Artifact folders are timestamped and kept, so rolling back is a redeploy:

```powershell
.\Deploy-Captive.ps1 -ArtifactPath C:\Captive\artifacts\<previous-timestamp>
```
