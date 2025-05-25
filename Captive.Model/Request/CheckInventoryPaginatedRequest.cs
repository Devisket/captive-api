using Captive.Model.Request.Interface;


namespace Captive.Model.Request
{
    public class CheckInventoryPaginatedRequest : IPaginatedRequest
    {
        public int Skip { get;set; }
        public int CurrentPage { get;set; }
        public List<Guid>? BranchId { get; set; }
        public List<Guid>? ProductIds { get; set; }
        public List<Guid>? FormCheckIds { get; set; }
        public bool isActivated { get; set; }
        public bool WarningSeries {  get; set; }
        public int PageSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
