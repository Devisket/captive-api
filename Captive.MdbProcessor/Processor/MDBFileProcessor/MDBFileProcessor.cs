using Captive.Model.Dto;
using Captive.Model.Processing.Configurations;
using System.Data.OleDb;
using System.Runtime.Versioning;

namespace Captive.Processing.Processor.MDBFileProcessor
{
    public class MDBFileProcessor : IMDBFileProcessor
    {
        /*
         * - Get configuraiton
         * - Map configuration with the predefined column
         * - Map records into CheckOrder
         * - Send check order back into FileProcessorConsumer
         */
        [SupportedOSPlatform("windows")]
        public IEnumerable<CheckOrderDto> Extractfile(OrderfileDto orderFile, MdbConfiguration config)
        {
            List<CheckOrderDto > checkOrders = new List<CheckOrderDto>();

            var filewatchingDir = Environment.GetEnvironmentVariable("CaptiveMDB");

            string strConnectionString =
                $"Provider='Microsoft.Jet.OLEDB.4.0';Data Source={orderFile.FilePath}" +
                ";Jet OLEDB:Database Password=captain" +
                ";Mode=Share Exclusive;Persist Security Info=True;";

            // Important part - using mdw file
            strConnectionString += "Jet OLEDB:System Database=" +
                    Environment.GetEnvironmentVariable("APPDATA") +
                    @"\Microsoft\Access\system.mdw";

            using (var connection = new OleDbConnection(strConnectionString))
            {
                try
                {
                    connection.Open();

                    // Create an OleDbCommand to retrieve data from a table
                    string query = $"SELECT * FROM {config.TableName}";

                    OleDbCommand command = new OleDbCommand(query, connection);

                    // Execute the command and read the results
                    OleDbDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        //Iterate on the configuration for every value we have
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return checkOrders;
        }
    }
}
