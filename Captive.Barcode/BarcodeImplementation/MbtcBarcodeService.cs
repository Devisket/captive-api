using Captive.Barcode.Base;
using Captive.Model.Dto.Reports;
using System.Diagnostics;

namespace Captive.Barcode.BarcodeImplementation
{
    public class MbtcBarcodeService : IBarcodeService
    {
        public string BarcodeImplementationName { get => "MbtcBarcode"; }

        public async Task<string> GenerateBarcode(CheckOrderReport param)
        {
            return await GenerateBarcodeAsync(param);
        }

        // Alternative method: Call console app asynchronously
        public async Task<string> GenerateBarcodeAsync(CheckOrderReport param)
        {
            string generatedBarcode = string.Empty;

            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Path\To\BarcodeGenerator.exe",
                    Arguments = $"\"{param.CheckOrder.AccountNo}\" \"{param.CheckOrder.BRSTN}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(processStartInfo))
                {
                    if (process != null)
                    {
                        // Asynchronously read output
                        string output = await process.StandardOutput.ReadToEndAsync();
                        string error = await process.StandardError.ReadToEndAsync();
                        
                        await process.WaitForExitAsync();
                        
                        if (process.ExitCode == 0)
                        {
                            generatedBarcode = output.Trim();
                        }
                        else
                        {
                            Console.WriteLine($"Async barcode generation failed: {error}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in async barcode generation: {ex.Message}");
            }

            return generatedBarcode;
        }
    }
}
