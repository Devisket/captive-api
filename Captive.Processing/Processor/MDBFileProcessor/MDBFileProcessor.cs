using Captive.Processing.Processor.Model;
using System.Data.Odbc;
using System.Data.OleDb;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Captive.Processing.Processor.MDBFileProcessor
{
    public class MDBFileProcessor : IFileProcessor
    {
        public ICollection<OrderFileData> OnProcessFile(byte[] file, string orderFileConfiguration)
        {
            try
            {
                OdbcCommand command = new OdbcCommand("");
                var connString = "Driver={Microsoft Access Driver (*.mdb, *.accdb)}; Dbq=C:\\CaptiveTest\\REG823.mdb123; Uid = Admin; Pwd =; ";
                using (OdbcConnection connection = new OdbcConnection(connString))
                {
                    command.Connection = connection;
                    connection.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            throw new NotImplementedException();
        }
    }
}
