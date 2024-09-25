
using System.Text.Json.Serialization;

namespace Captive.Model.Processing.Configurations
{
    public class MdbConfiguration
    {
        [JsonPropertyName("hasPassword")]
        public bool HasPassword { get; set; }

        [JsonPropertyName("hasPassword")]
        public bool HasBarcode { get; set; }

        [JsonPropertyName("tableName")]
        public string TableName {  get; set; }

        [JsonPropertyName("columnDefinition")]
        public List<MdbColumnDefinition> ColumnDefinition { get; set; }
    }

    public class MdbColumnDefinition
    {
        [JsonPropertyName("fieldName")]
        public string FieldName { get; set; }

        [JsonPropertyName("columnName")]
        public string ColumnName { get; set; }
    }
}
