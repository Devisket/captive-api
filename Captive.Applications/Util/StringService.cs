using System.Text.RegularExpressions;

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
    }
}
