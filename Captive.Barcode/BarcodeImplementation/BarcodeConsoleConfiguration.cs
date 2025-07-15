namespace Captive.Barcode.BarcodeImplementation
{
    public class BarcodeConsoleConfiguration
    {
        public string ExecutablePath { get; set; } = @"C:\BarcodeGenerator\BarcodeGenerator.exe";
        public string WorkingDirectory { get; set; } = @"C:\BarcodeGenerator";
        public int TimeoutSeconds { get; set; } = 30;
        public bool ShowConsoleWindow { get; set; } = false;
        public string ArgumentTemplate { get; set; } = "\"{0}\" \"{1}\" \"{2}\" \"{3}\""; // AccountNo, BRSTN, StartSeries, EndSeries
    }
} 