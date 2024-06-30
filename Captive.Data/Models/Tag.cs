using Captive.Data.Enums;
namespace Captive.Data.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string TagName { get; set;}
        public TagType TagType { get; set;}
    }
}
