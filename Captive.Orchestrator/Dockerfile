FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Captive.Orchestrator/Captive.Orchestrator.csproj", "Captive.Orchestrator/"]
COPY ["Captive.Messaging/Captive.Messaging.csproj", "Captive.Messaging/"]
COPY ["Captive.Model/Captive.Model.csproj", "Captive.Model/"]
COPY ["Captive.Reports/Captive.Reports.csproj", "Captive.Reports/"]
COPY ["Captive.Utility/Captive.Utility.csproj", "Captive.Utility/"]
RUN dotnet restore "./Captive.Orchestrator/Captive.Orchestrator.csproj"
COPY . .
WORKDIR "/src/Captive.Orchestrator"
RUN dotnet publish "./Captive.Orchestrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Captive.Orchestrator.dll"]
