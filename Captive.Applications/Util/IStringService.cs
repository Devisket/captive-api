
namespace Captive.Applications.Util
{
    public interface IStringService
    {
        string GetInitialSeries(string pattern);
        Tuple<string, string> GetNextSeries(string pattern, string lastSeries, int quantity);
    }
}
