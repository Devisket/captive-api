﻿using System.Text.RegularExpressions;

namespace Captive.Applications.Util
{
    public class StringService : IStringService
    {
        public string GetInitialSeries(string pattern)
        {
            var initialValue = 0;
            
            var paddingCount = pattern.Count(x => x == '0');
            var seriesValue = initialValue.ToString().PadLeft(paddingCount, '0');

            return string.Concat(pattern.Replace("0", string.Empty), seriesValue);
        }

        public Tuple<string, string> GetNextSeries(string pattern, string lastSeries, int quantity)
        {
            var paddingCount = pattern.Count(x => x == '0');
            int numValue = string.IsNullOrEmpty(lastSeries) ?  0 : Convert.ToInt32(Regex.Match(lastSeries, "\\d{5}$").Value);
            Tuple<string, string> returnObj = new Tuple<string, string>(string.Concat(pattern.Replace("0", string.Empty), (numValue + 1).ToString().PadLeft(paddingCount, '0')),
                string.Concat(pattern.Replace("0", string.Empty), (numValue + quantity).ToString().PadLeft(paddingCount, '0')));

            return returnObj;
        }

        public Tuple<int, int> ExtractNumber(string seriesPattern, string startingSeries, string endingSeries)
        {
            var startingNumber = startingSeries.Replace(string.Empty, seriesPattern);
            var endingNumber = endingSeries.Replace(string.Empty, seriesPattern);

            var a = int.Parse(startingNumber);
            var b = int.Parse(endingNumber);

            return new Tuple<int, int>(a, b);
        }

        public Tuple<string, string> ConvertToSeries(string seriesPattern, int numberOfPadding, int startingNumber, int endingNumber)
        {
            var startingSeries = Convert.ToString(startingNumber).PadLeft(numberOfPadding);
            var endingSeries = Convert.ToString(endingNumber).PadLeft(numberOfPadding);

            startingSeries = String.Concat(seriesPattern, startingSeries);
            endingSeries = String.Concat(seriesPattern, endingSeries);

            return new Tuple<string, string>(startingSeries, endingSeries);
        }
    }
}
