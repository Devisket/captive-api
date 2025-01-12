
namespace Captive.Applications.Util
{
    public interface IStringService
    {
        string GetInitialSeries(string pattern);
        Tuple<string, string> GetNextSeries(string pattern, string lastSeries, int quantity);

        Tuple<int, int> ExtractNumber(string seriesPattern, string startingSeries, string endingSeries);
        Tuple<string, string> ConvertToSeries(string seriesPattern, int numberOfPadding, int startingNumber, int endingNumber);
    }
}
