using System;
using BcConfig;

namespace BarcodeGenerator
{
    internal class Program
    {
        /*
         * - Account Number
         * - BRSTN
         * - Check Serial
         */
        static void Main(string[] args)
        {
            string generatedBarcode = string.Empty;

            clsBcConfig clsBcConfig = new clsBcConfig();
            //For Barcode
            clsBcConfig.AccountNo = args[0];
            clsBcConfig.BRSTN = args[1];
            clsBcConfig.CheckSerial = args[2];
            clsBcConfig.set_ConfigPath(AppDomain.CurrentDomain.BaseDirectory);
            //End For Barcode

            string barcodedata = clsBcConfig.BarCode;

            if (!string.IsNullOrEmpty(barcodedata))
                generatedBarcode = barcodedata;

            Console.WriteLine(generatedBarcode);
        }
    }
}
