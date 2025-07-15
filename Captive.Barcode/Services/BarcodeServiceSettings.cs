namespace Captive.Barcode.Services
{
    public class BarcodeServiceSettings
    {
        public int HeartbeatIntervalSeconds { get; set; } = 30;
        public string DefaultBarcodeImplementation { get; set; } = "MbtcBarcode";
        public string ConsoleAppPath { get; set; } = @"C:\BarcodeGenerator\BarcodeGenerator.exe";
        public string WorkingDirectory { get; set; } = @"C:\BarcodeGenerator";
        public int ProcessTimeoutSeconds { get; set; } = 30;
        public bool ShowConsoleWindow { get; set; } = false;
        public bool EnableDetailedLogging { get; set; } = false;
        public string TempDirectory { get; set; } = @"C:\Temp\BarcodeService";
    }
} 