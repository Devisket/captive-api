using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Model.Dto.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.OleDb;

namespace Captive.MdbProcessor.Processor.DbfGenerator
{
    public interface IDbfGenerator
    {
        Task GenerateDbf(List<OrderFile> orderFiles, CancellationToken cancellationToken);
    }
    public class DbfGenerator : IDbfGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly IReadUnitOfWork _readUow;
        private readonly IWriteUnitOfWork _writeUow;

        public DbfGenerator(IConfiguration configuration, IReadUnitOfWork readUow, IWriteUnitOfWork writeUow)
        {
            _configuration = configuration;
            _readUow = readUow;
            _writeUow = writeUow;
        }

        public async Task GenerateDbf(List<OrderFile> orderFiles, CancellationToken cancellationToken)
        {
            var bankInfo = orderFiles.First().BatchFile.BankInfo;

            try
            {
                foreach (var orderFile in orderFiles) 
                {
                    var fileDirectory = CreateDirectory(bankInfo.ShortName, orderFiles.First().BatchFile.BatchName);

                    var productNames = orderFile.Product.ProductName;

                    fileDirectory = Path.Combine(fileDirectory, productNames);

                    if (!Directory.Exists(fileDirectory)) 
                    { 
                        Directory.CreateDirectory(fileDirectory);
                    }

                    // Delete existing DBF file if it exists
                    var dbfFilePath = Path.Combine(fileDirectory, "dbf_file.dbf");
                    if (File.Exists(dbfFilePath))
                    {
                        File.Delete(dbfFilePath);
                    }

                    string strConnectionString = $"Provider='Microsoft.Jet.OLEDB.4.0';Data Source={fileDirectory};" + "Extended Properties=dBase IV";

                    var connection = new OleDbConnection(strConnectionString);

                    await connection.OpenAsync();

                    var transaction = connection.BeginTransaction();

                    OleDbCommand cmd = new OleDbCommand("Create Table dbf_file (BATCHNO varchar(50), BLOCK integer, RT_NO varchar(9), M varchar(1), BRANCH varchar(50), ACCT_NO varchar(15), ACCT_NO_P varchar(16), CHKTYPE varchar(50), ACCT_NAME1 varchar(100), " +
                                                "ACCT_NAME2 varchar(100), NO_BKS integer, CK_NO_P integer, CK_NO_B varchar(10), CK_NOE integer, CK_NO_E varchar(10), DELIVERTO varchar(30))", connection, transaction);

                    cmd.ExecuteNonQuery();

                    transaction.Commit();

                    await InsertDbfRecord(connection, orderFile, cancellationToken);

                    await connection.CloseAsync();

                    orderFile.Status = OrderFilesStatus.Completed;
                }

                _writeUow.OrderFiles.UpdateRange(orderFiles);

                await _writeUow.Complete();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task InsertDbfRecord(OleDbConnection connection, OrderFile orderFile, CancellationToken cancellationToken ) 
        {
            var batch = orderFile.BatchFile;
            
            var formChecks = await _readUow.FormChecks.GetAll().Where(x => x.ProductId == orderFile.ProductId).Select(x => new {x.Id, x.FormCheckType}).ToListAsync();

            var branches = _readUow.BankBranches.GetAll().Include(x => x.BankInfo).Where(x => x.BankInfoId ==  orderFile.BatchFile.BankInfoId);

            var accountNumberFormat = branches.First().BankInfo.AccountNumberFormat;

            var checkOrders = await ExtractCheckOrderDto(orderFile.CheckOrders!, batch!.BankInfoId, cancellationToken);

            foreach (var checkOrder in checkOrders.OrderBy(x => x.CheckTypeLetter).ThenBy(x => x.CheckOrder.BRSTN).ThenBy(x => x.CheckOrder.AccountNo).ThenBy(x => x.StartSeries)) 
            {
                string commandInsert = "INSERT INTO dbf_file (BATCHNO, BLOCK , RT_NO , BRANCH, ACCT_NO, ACCT_NO_P, CHKTYPE, ACCT_NAME1, ACCT_NAME2, CK_NO_P, CK_NO_B, CK_NO_E, DELIVERTO) " +
                                   "VALUES (@batchNo, @block, @rtNo, @branch, @acctNo, @acctNoP, @checkType, @accName1, @accName2, @ckNoP, @ckNoB, @ckNoE, @deliverTo)";

                var transaction = connection.BeginTransaction();

                var command = new OleDbCommand(commandInsert, connection, transaction);

                var formattedAccNo = FormatAccountNumber(checkOrder.CheckOrder.AccountNo, accountNumberFormat);

                var formCheckTypeString = checkOrder.FormCheckType == FormCheckType.Personal ? "A" : "B";

                var deliverTo = checkOrder.CheckOrder.DeliverTo ?? string.Empty;

                command.Parameters.AddWithValue("@batchNo", orderFile.FileName);
                command.Parameters.AddWithValue("@block", 0);
                command.Parameters.AddWithValue("@rtNo", checkOrder.CheckOrder.BRSTN);
                command.Parameters.AddWithValue("@branch", $"{checkOrder.BankBranch.BranchName} BRANCH({checkOrder.BankBranch.BranchCode})");
                command.Parameters.AddWithValue("@acctNo", checkOrder.CheckOrder.AccountNo);
                command.Parameters.AddWithValue("@acctNoP", formattedAccNo);
                command.Parameters.AddWithValue("@checkType", formCheckTypeString);
                command.Parameters.AddWithValue("@accName1", checkOrder.CheckOrder.AccountName);
                command.Parameters.AddWithValue("@accName2", string.Empty);
                command.Parameters.AddWithValue("@ckNoP", 0);
                command.Parameters.AddWithValue("@ckNoB", checkOrder.StartSeries);
                command.Parameters.AddWithValue("@ckNoE", checkOrder.EndSeries);
                command.Parameters.AddWithValue("@deliverTo", deliverTo);

                command.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        private async Task<ICollection<CheckOrderReport>> ExtractCheckOrderDto(ICollection<CheckOrders> checkOrders, Guid bankId, CancellationToken cancellationToken)
        {
            var branches = await GetAlLBranches(bankId, cancellationToken);

            var returnDatas = new List<CheckOrderReport>();

            var formChecks = await GetFormChecks(checkOrders.GroupBy(x => x.FormCheckId ?? Guid.Empty).Select(x => x.Key).ToList());

            foreach (var checkOrder in checkOrders)
            {
                var checkInventory = await GetCheckInventory(checkOrder.Id, cancellationToken);

                var branch = branches.First(x => x.BRSTNCode == checkOrder.BRSTN);

                var formCheck = formChecks.First(x => x.Id == checkOrder.FormCheckId);

                foreach (var check in checkInventory)
                {
                    returnDatas.Add(new CheckOrderReport
                    {
                        ProductTypeName = formCheck.Product.ProductName,
                        FormCheckName = formCheck.Description,
                        FileInitial = formCheck.FileInitial,
                        CheckOrder = checkOrder,
                        BankBranch = branch,
                        FormCheckType = formCheck.FormCheckType,
                        OrderFileName = checkOrder.OrderFile.FileName,
                        CheckTypeLetter = formCheck.FormCheckType == FormCheckType.Personal ? "A" : "B",
                        CheckInventoryId = check.Id,
                        StartSeries = check.StartingSeries ?? string.Empty,
                        EndSeries = check.EndingSeries ?? string.Empty,
                        AccountNumberFormat = branches.First().BankInfo.AccountNumberFormat,
                    });
                }
            }

            return returnDatas;
        }


        public async Task<ICollection<BankBranches>> GetAlLBranches(Guid bankId, CancellationToken cancellationToken)
        {
            var bankBranches = await _readUow.BankBranches.GetAll()
                .Include(x => x.BankInfo)
                .Where(x => x.BankInfoId == bankId)
                .AsNoTracking()
                .ToListAsync();

            return bankBranches;
        }

        public async Task<ICollection<CheckInventoryDetail>> GetCheckInventory(Guid checkOrderId, CancellationToken cancellationToken)
        {
            var checkInventory = await _readUow.CheckInventoryDetails.GetAll()
               .AsNoTracking()
               .Where(x => x.CheckOrderId == checkOrderId)
               .ToListAsync();

            return checkInventory;
        }

        private async Task<ICollection<FormChecks>> GetFormChecks(List<Guid> formCheckIds)
        {
            var formCheck = await _readUow.FormChecks.GetAll()
                .Include(x => x.Product)
                .AsNoTracking()
                .Where(x => formCheckIds.Any(z => z == x.Id))
                .AsNoTracking()
                .ToListAsync();

            return formCheck;
        }

        private string CreateDirectory(string bankShortName, string batchName)
        {
            var rootPath = _configuration["Processing:OutputFile"];

            rootPath = rootPath.Replace("bankShortName", bankShortName);
            rootPath = rootPath.Replace("currentDate", DateTime.UtcNow.ToString("MM-dd-yyyy"));
            rootPath = rootPath.Replace("batchName", batchName);


            if (string.IsNullOrEmpty(rootPath))
                throw new Exception("File processing directory is not defined");

            Directory.CreateDirectory(rootPath);

            return rootPath;
        }

        private string FormatAccountNumber(string accountNumber, string format)
        {
            if (string.IsNullOrEmpty(accountNumber) || string.IsNullOrEmpty(format))
                return accountNumber;

            var segments = format.Split('-');
            var result = new List<string>();
            var currentIndex = 0;

            foreach (var segment in segments)
            {
                var segmentLength = segment.Length;
                if (currentIndex + segmentLength <= accountNumber.Length)
                {
                    result.Add(accountNumber.Substring(currentIndex, segmentLength));
                    currentIndex += segmentLength;
                }
                else if (currentIndex < accountNumber.Length)
                {
                    result.Add(accountNumber.Substring(currentIndex));
                    break;
                }
            }

            return string.Join("-", result);
        }
    }
}
