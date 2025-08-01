using System.Text.RegularExpressions;

namespace Captive.Applications.Util
{
    public interface IStringService
    {
        string GetInitialSeries(string pattern);
        Tuple<string, string> GetNextSeries(string pattern, string lastSeries, int quantity);
        Tuple<long, long> ExtractNumber(string seriesPattern, string startingSeries, string endingSeries);
        Tuple<string, string> ConvertToSeries(string seriesPattern, int numberOfPadding, long startingNumber, long endingNumber);
    }
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
            long numValue = string.IsNullOrEmpty(lastSeries) ?  0 : Convert.ToInt32(Regex.Match(lastSeries, "\\d{5}$").Value);
            Tuple<string, string> returnObj = new Tuple<string, string>(string.Concat(pattern.Replace("0", string.Empty), (numValue + 1).ToString().PadLeft(paddingCount, '0')),
                string.Concat(pattern.Replace("0", string.Empty), (numValue + quantity).ToString().PadLeft(paddingCount, '0')));

            return returnObj;
        }

        public Tuple<long, long> ExtractNumber(string seriesPattern, string startingSeries, string endingSeries)
        {
            long a, b = 0;

            if (!String.IsNullOrEmpty(seriesPattern))
            {
                var startingNumber = startingSeries.Replace(seriesPattern, string.Empty);
                var endingNumber = endingSeries.Replace(seriesPattern, string.Empty);

                a = long.Parse(startingNumber);
                b = long.Parse(endingNumber);

                return new Tuple<long, long>(a, b);
            }

            a = long.Parse(startingSeries);
            b = long.Parse(endingSeries);

            return new Tuple<long, long>(a, b);
        }

        public Tuple<string, string> ConvertToSeries(string seriesPattern, int numberOfPadding, long startingNumber, long endingNumber)
        {
            var startingSeries = Convert.ToString(startingNumber).PadLeft(numberOfPadding,'0');
            var endingSeries = Convert.ToString(endingNumber).PadLeft(numberOfPadding,'0');

            startingSeries = String.Concat(seriesPattern, startingSeries);
            endingSeries = String.Concat(seriesPattern, endingSeries);

            return new Tuple<string, string>(startingSeries, endingSeries);
        }
    }
}
