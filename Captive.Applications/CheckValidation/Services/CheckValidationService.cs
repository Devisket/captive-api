using Captive.Applications.Util;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Applications.CheckValidation.Services
{
    
    public interface ICheckValidationService
    {
        Task<bool> HasConflictedSeries(Guid CheckInventoryId, string startingSeries, string endingSeries, CancellationToken cancellationToken)
    }
    public class CheckValidationService : ICheckValidationService
    {
        private readonly IReadUnitOfWork _readUow;
        private readonly IStringService _stringService;

        public CheckValidationService(IReadUnitOfWork readUow, IStringService stringService) 
        { 
            _readUow = readUow;
            _stringService = stringService;
        }

        public async Task<bool>HasConflictedSeries(Guid CheckInventoryId, string startingSeries, string endingSeries, CancellationToken cancellationToken)
        {
            var checkInventory = await _readUow.CheckInventory
                .GetAll()
                .Include(x => x.CheckInventoryDetails)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (checkInventory.CheckInventoryDetails == null || !checkInventory.CheckInventoryDetails.Any())
                return false;

            var checkInventoryDetails = checkInventory.CheckInventoryDetails.ToList();

            var numberSeries = _stringService.ExtractNumber(checkInventory.SeriesPatern, startingSeries, endingSeries);

            return checkInventoryDetails.Any(x =>
            (x.EndingNumber >= numberSeries.Item1 && x.StartingNumber <= numberSeries.Item1) ||
            (x.EndingNumber >= numberSeries.Item2 && x.StartingNumber <= numberSeries.Item2));
        }

    }
}
