
namespace Captive.Model.Request.Interface
{
    public interface IPaginatedRequest
    {
        int PageSize{ get; set; }
        int CurrentPage { get; set; }
    }
}
