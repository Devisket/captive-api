using Captive.Data.Models;
using Captive.Data.UnitOfWork.Read;
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

        public DbfGenerator(IConfiguration configuration, IReadUnitOfWork readUow)
        {
            _configuration = configuration;
            _readUow = readUow;
        }

        public async Task GenerateDbf(List<OrderFile> orderFiles, CancellationToken cancellationToken)
        {
            var bankInfo = await _readUow.Banks.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.Id == orderFiles.First().BatchFile.BankInfoId, cancellationToken);           

            var fileDirectory = CreateDirectory(bankInfo.ShortName, orderFiles.First().BatchFile.BatchName);

            try
            {
                foreach (var orderFile in orderFiles) 
                {
                    var productNames = orderFile.Product.ProductName;

                    fileDirectory = Path.Combine(fileDirectory, productNames);

                    if (!File.Exists(fileDirectory)) 
                    { 
                        Directory.CreateDirectory(fileDirectory);
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
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task InsertDbfRecord(OleDbConnection connection, OrderFile orderFile, CancellationToken cancellationToken ) 
        {
            var batch = orderFile.BatchFile;

            var checkOrders = orderFile.CheckOrders;
            
            var formChecks = await _readUow.FormChecks.GetAll().Where(x => x.ProductId == orderFile.ProductId).Select(x => new {x.Id, x.FormCheckType}).ToListAsync();

            foreach (var checkOrder in checkOrders) 
            {                
                var checkInventoryDetails = checkOrder.CheckInventoryDetail.OrderBy(x => x.StartingNumber).ToList();

                foreach (var checkInventoryDetail in checkInventoryDetails)
                {
                    string commandInsert = "INSERT INTO dbf_file (BATCHNO, BLOCK , RT_NO , BRANCH, ACCT_NO, ACCT_NO_P, CHKTYPE, ACCT_NAME1, ACCT_NAME2, CK_NO_P, CK_NO_B, CK_NO_E, DELIVERTO) " +
                                   "VALUES (@batchNo, @block, @rtNo, @branch, @acctNo, @acctNoP, @checkType, @accName1, @accName2, @ckNoP, @ckNoB, @ckNoE, @deliverTo)";

                    var transaction = connection.BeginTransaction();

                    var command = new OleDbCommand(commandInsert, connection, transaction);

                    command.Parameters.AddWithValue("@batchNo", batch.BatchName);
                    command.Parameters.AddWithValue("@block", 0);
                    command.Parameters.AddWithValue("@rtNo", checkOrder.BRSTN);
                    command.Parameters.AddWithValue("@branch", checkOrder.BRSTN);
                    command.Parameters.AddWithValue("@acctNo", checkOrder.AccountNo);
                    command.Parameters.AddWithValue("@acctNoP", checkOrder.AccountNo);
                    command.Parameters.AddWithValue("@checkType", formChecks.First(x => x.Id == checkOrder.FormCheckId).FormCheckType);
                    command.Parameters.AddWithValue("@accName1", checkOrder.AccountName);
                    command.Parameters.AddWithValue("@accName2", string.Empty);
                    command.Parameters.AddWithValue("@ckNoP", 0);
                    command.Parameters.AddWithValue("@ckNoB", checkInventoryDetail.StartingSeries);
                    command.Parameters.AddWithValue("@ckNoE", checkInventoryDetail.EndingSeries);
                    command.Parameters.AddWithValue("@deliverTo", checkOrder.DeliverTo);

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
    }
}
