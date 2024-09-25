using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Captive.Utility
{
    public static class StringHelper
    {
        public static string GenerateRandomChar(int length)
        {
            var random = new Random();
            char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

            var stringVal = string.Empty;
            for (int i = 0; i < length; i++) 
            {
                var randomNum = random.Next(1, chars.Length);
                stringVal += chars[randomNum];
            }
            
            return stringVal;
        }

        public static string SanitizeFileName(this string fileName)
        {
            // Regular expression pattern to match numbers in the file extension
            string pattern = @"\d+";

            // Replace the numbers in the file extension with an empty string
            string result = Regex.Replace(fileName, pattern, String.Empty);

            return result;
        }
    }
}
