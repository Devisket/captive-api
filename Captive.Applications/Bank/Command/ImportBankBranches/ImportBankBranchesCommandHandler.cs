using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Captive.Applications.Bank.Command.ImportBankBranches
{
    public class ImportBankBranchesCommandHandler : IRequestHandler<ImportBankBranchesCommand, ImportBankBranchResult>
    {
        private readonly IWriteUnitOfWork _writeUow;

        public ImportBankBranchesCommandHandler(IWriteUnitOfWork writeUow)
        {
            _writeUow = writeUow;
        }

        public async Task<ImportBankBranchResult> Handle(ImportBankBranchesCommand request, CancellationToken cancellationToken)
        {
            var result = new ImportBankBranchResult();

            List<BankBranchCsvRow> rows;
            try
            {
                rows = ParseCsv(request.FileStream);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Failed to parse file: {ex.Message}");
                return result;
            }

            if (rows.Count == 0)
            {
                result.Errors.Add("File is empty or contains no valid rows.");
                return result;
            }

            var existingBranches = await _writeUow.BankBranches.GetAll()
                .Where(x => x.BankInfoId == request.BankId)
                .ToListAsync(cancellationToken);

            foreach (var (row, index) in rows.Select((r, i) => (r, i + 2)))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(row.BRSTNCode))
                    {
                        result.Errors.Add($"Row {index}: BRSTNCode is required.");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(row.BranchName))
                    {
                        result.Errors.Add($"Row {index}: BranchName is required.");
                        continue;
                    }

                    BranchStatus status = BranchStatus.Active;
                    if (!string.IsNullOrWhiteSpace(row.BranchStatus) &&
                        !Enum.TryParse(row.BranchStatus.Trim(), ignoreCase: true, out status))
                    {
                        result.Errors.Add($"Row {index}: BranchStatus '{row.BranchStatus}' is invalid. Accepted values: Active, Closing, Inactive.");
                        continue;
                    }

                    var existing = existingBranches.FirstOrDefault(b =>
                        b.BRSTNCode.Trim().Equals(row.BRSTNCode.Trim(), StringComparison.OrdinalIgnoreCase));

                    if (existing != null)
                    {
                        existing.BranchName = row.BranchName.Trim();
                        existing.BRSTNCode = row.BRSTNCode.Trim();
                        existing.BranchCode = row.BranchCode?.Trim() ?? string.Empty;
                        existing.BranchAddress1 = row.BranchAddress1?.Trim();
                        existing.BranchAddress2 = row.BranchAddress2?.Trim();
                        existing.BranchAddress3 = row.BranchAddress3?.Trim();
                        existing.BranchAddress4 = row.BranchAddress4?.Trim();
                        existing.BranchAddress5 = row.BranchAddress5?.Trim();
                        existing.BranchStatus = status;
                        _writeUow.BankBranches.Update(existing);
                        result.Updated++;
                    }
                    else
                    {
                        var newBranch = new BankBranches
                        {
                            Id = Guid.NewGuid(),
                            BankInfoId = request.BankId,
                            BRSTNCode = row.BRSTNCode.Trim(),
                            BranchName = row.BranchName.Trim(),
                            BranchCode = row.BranchCode?.Trim() ?? string.Empty,
                            BranchAddress1 = row.BranchAddress1?.Trim(),
                            BranchAddress2 = row.BranchAddress2?.Trim(),
                            BranchAddress3 = row.BranchAddress3?.Trim(),
                            BranchAddress4 = row.BranchAddress4?.Trim(),
                            BranchAddress5 = row.BranchAddress5?.Trim(),
                            BranchStatus = status,
                        };
                        await _writeUow.BankBranches.AddAsync(newBranch, cancellationToken);
                        existingBranches.Add(newBranch);
                        result.Created++;
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {index}: {ex.Message}");
                }
            }

            if (result.Created > 0 || result.Updated > 0)
                await _writeUow.Complete();

            return result;
        }

        private static List<BankBranchCsvRow> ParseCsv(Stream stream)
        {
            using var reader = new StreamReader(stream, leaveOpen: true);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim,
            };
            using var csv = new CsvReader(reader, config);
            return csv.GetRecords<BankBranchCsvRow>().ToList();
        }
    }
}
