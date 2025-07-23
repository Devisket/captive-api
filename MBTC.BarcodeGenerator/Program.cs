using System;
using System.Collections.Generic;
using BcConfig;

namespace BarcodeGenerator
{
    internal class Program
    {
        /*
         * - Account Number
         * - BRSTN
         * - Check Serial (can be multiple series separated by semicolons)
         */
        static void Main(string[] args)
        {
            var series = args[2].Split(';');
            var generatedBarcodes = new List<string>();

            foreach (var singleSeries in series)
            {
                clsBcConfig clsBcConfig = new clsBcConfig();
                //For Barcode
                clsBcConfig.AccountNo = args[0];
                clsBcConfig.BRSTN = args[1];
                clsBcConfig.CheckSerial = singleSeries.Trim(); // Use individual series
                clsBcConfig.set_ConfigPath(AppDomain.CurrentDomain.BaseDirectory);
                //End For Barcode

                string barcodedata = clsBcConfig.BarCode;

                if (!string.IsNullOrEmpty(barcodedata))
                {
                    generatedBarcodes.Add(barcodedata);
                }
            }

            // Join all generated barcodes with semicolons
            var result = string.Join(";", generatedBarcodes);
            Console.WriteLine(result);
        }
    }
}
