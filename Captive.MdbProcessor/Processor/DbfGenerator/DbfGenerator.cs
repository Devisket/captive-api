using Captive.Data.Enums;
using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
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

            var checkOrders = orderFile.CheckOrders.OrderBy(x => x.BRSTN).ThenBy(x => x.AccountNo);
            
            var formChecks = await _readUow.FormChecks.GetAll().Where(x => x.ProductId == orderFile.ProductId).Select(x => new {x.Id, x.FormCheckType}).ToListAsync();

            var branches = _readUow.BankBranches.GetAll().Include(x => x.BankInfo).Where(x => x.BankInfoId ==  orderFile.BatchFile.BankInfoId);

            foreach (var checkOrder in checkOrders) 
            {                
                var checkInventoryDetails = checkOrder.CheckInventoryDetail.OrderBy(x => x.StartingSeries).ToList();

                foreach (var checkInventoryDetail in checkInventoryDetails)
                {
                    string commandInsert = "INSERT INTO dbf_file (BATCHNO, BLOCK , RT_NO , BRANCH, ACCT_NO, ACCT_NO_P, CHKTYPE, ACCT_NAME1, ACCT_NAME2, CK_NO_P, CK_NO_B, CK_NO_E, DELIVERTO) " +
                                   "VALUES (@batchNo, @block, @rtNo, @branch, @acctNo, @acctNoP, @checkType, @accName1, @accName2, @ckNoP, @ckNoB, @ckNoE, @deliverTo)";

                    var transaction = connection.BeginTransaction();

                    var command = new OleDbCommand(commandInsert, connection, transaction);

                    var branch = branches.First(x => x.Id == checkOrder.BranchId);

                    var accountNumberFormat = branch.BankInfo.AccountNumberFormat;

                    var formattedAccNo = FormatAccountNumber(checkOrder.AccountNo, accountNumberFormat);

                    var formCheckType = formChecks.First(x => x.Id == checkOrder.FormCheckId).FormCheckType;

                    var formCheckTypeString = formCheckType == FormCheckType.Personal ? "A" : "B";

                    var deliverTo = checkOrder.DeliverTo ??  string.Empty;

                    command.Parameters.AddWithValue("@batchNo", orderFile.FileName);
                    command.Parameters.AddWithValue("@block", 0);
                    command.Parameters.AddWithValue("@rtNo", checkOrder.BRSTN);
                    command.Parameters.AddWithValue("@branch", $"{branch.BranchName} BRANCH({branch.BranchCode})");
                    command.Parameters.AddWithValue("@acctNo", checkOrder.AccountNo);
                    command.Parameters.AddWithValue("@acctNoP", formattedAccNo);
                    command.Parameters.AddWithValue("@checkType", formCheckTypeString);
                    command.Parameters.AddWithValue("@accName1", checkOrder.AccountName);
                    command.Parameters.AddWithValue("@accName2", string.Empty);
                    command.Parameters.AddWithValue("@ckNoP", 0);
                    command.Parameters.AddWithValue("@ckNoB", checkInventoryDetail.StartingSeries);
                    command.Parameters.AddWithValue("@ckNoE", checkInventoryDetail.EndingSeries);
                    command.Parameters.AddWithValue("@deliverTo", deliverTo);

                    command.ExecuteNonQuery();

                    transaction.Commit();
                }  
            }
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
