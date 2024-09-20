using Captive.Processing.Processor.Model;
using System.Data.OleDb;

namespace Captive.Processing.Processor.MDBFileProcessor
{
    public class MDBFileProcessor : IMDBFileProcessor
    {
        public IEnumerable<OrderFileData> Extractfile(Guid batchId, string fileName, string config)
        {
            var filewatchingDir = Environment.GetEnvironmentVariable("CaptiveMDB");

            string strConnectionString =
                "Provider='Microsoft.Jet.OLEDB.4.0';Data Source=C:\\CaptiveTest\\MDT999.mdb123" +
                ";Jet OLEDB:Database Password=captain" +
                ";Mode=Share Exclusive;Persist Security Info=True;";

            // Important part - using mdw file
            strConnectionString += "Jet OLEDB:System Database=" +
                    Environment.GetEnvironmentVariable("APPDATA") +
                    @"\Microsoft\Access\system.mdw";

            string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\CaptiveTest\MDT999.mdb123;Password=captain";

            using (var connection = new OleDbConnection(strConnectionString))
            {
                try
                {
                    connection.Open();

                    // Create an OleDbCommand to retrieve data from a table
                    string query = "SELECT * FROM ChkBook";
                    OleDbCommand command = new OleDbCommand(query, connection);

                    // Execute the command and read the results
                    OleDbDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        // Access data using reader (e.g., reader["ColumnName"])
                        Console.WriteLine(reader["ColumnName"].ToString());
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            throw new NotImplementedException();
        }
    }
}
