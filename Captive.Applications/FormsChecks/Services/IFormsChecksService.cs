using Captive.Model.Dto;
namespace Captive.Applications.FormsChecks.Services
{
    public interface IFormsChecksService
    {
        Task<Captive.Data.Models.FormChecks?> GetCheckOrderFormCheck(CheckOrderDto checkOrder, CancellationToken cancellationToken);
    }
}
