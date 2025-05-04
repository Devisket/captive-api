using Captive.Model.Processing.Configurations.Interfaces;
using System.Text.Json.Serialization;

namespace Captive.Model.Processing.Configurations
{
    public class DbfConfiguration : IPassword, ITable
    {
        [JsonPropertyName("hasPassword")]
        public bool HasPassword { get; set; }

        [JsonPropertyName("hasPassword")]
        public bool HasBarcode { get; set; }

        [JsonPropertyName("tableName")]
        public string TableName { get; set; } = string.Empty;

        [JsonPropertyName("IsEncrypted")]
        public bool IsEncrypted { get; set; }

        [JsonPropertyName("Password")]
        public string? Password { get; set; }

        [JsonPropertyName("columnDefinitions")]
        public List<DbfDefinition> ColumnDefinitions { get; set; }

        public Dictionary<string, string> ToDictionary()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var columnDefinition in ColumnDefinitions)
                dictionary.Add(columnDefinition.FieldName, columnDefinition.ColumnName);

            return dictionary;
        }
    }

    public class DbfDefinition
    {
        [JsonPropertyName("fieldName")]
        public string FieldName { get; set; }

        [JsonPropertyName("columnName")]
        public string ColumnName { get; set; }
    }
}
