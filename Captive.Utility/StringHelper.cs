using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Utility
{
    public class StringHelper
    {
        public string GenerateRandomChar(int length)
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
    }
}
