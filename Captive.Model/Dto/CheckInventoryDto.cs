using Captive.Data.Models;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Captive.Model.Dto
{
    public class CheckInventoryDto
    {
        public Guid? Id { get; set; }
        public Guid TagId { get; set; }
        public string? SeriesPattern { get; set; }
        public int WarningSeries { get; set; }
        public int NumberOfPadding {  get; set; }
        public int StartingSeries {  get; set; }
        public int EndingSeries { get; set; }
        public int CurrentSeries {  get; set; }
        public bool IsRepeating {  get; set; }
        public bool IsActive { get; set; }
        public CheckInventoryMappingData? MappingData { get; set; }
        public static CheckInventoryDto ToDto(CheckInventory input)
        {
            return new CheckInventoryDto
            {
                Id = input.Id,
                TagId = input.TagId,
                WarningSeries = input.WarningSeries,
                NumberOfPadding = input.NumberOfPadding,
                StartingSeries = input.StartingSeries,
                EndingSeries = input.EndingSeries,
                CurrentSeries = input.CurrentSeries,
                IsActive = input.IsActive,
                MappingData = String.IsNullOrEmpty(input.JsonMappingData) ? new CheckInventoryMappingData() : JsonConvert.DeserializeObject<CheckInventoryMappingData>(input.JsonMappingData),
                IsRepeating = input.isRepeating,
                SeriesPattern = input.SeriesPatern
            };
        }
    }
}
