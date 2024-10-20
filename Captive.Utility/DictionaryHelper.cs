

namespace Captive.Utility
{
    public static class DictionaryHelper
    {
        public static string GetFieldValue(this Dictionary<string, object?> dictionary, string keyName) => dictionary.ContainsKey(keyName) ? dictionary[keyName].ToString() : string.Empty;
    }
}
