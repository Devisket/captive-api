using Captive.Data.Models;
using Captive.Data;
using Microsoft.EntityFrameworkCore;
using Captive.Data.Enums;

namespace Captive.Commands.Extensions
{
    public static class SeedDatabase
    {
        public static async Task SeetData(this WebApplication webApplication, ILogger logger, IConfiguration configuration)
        {
            try
            {
                using (var scope = webApplication.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<CaptiveDataContext>();

                    if (!await context.Set<BankInfo>().AnyAsync())
                    {
                        logger.LogInformation("Seeding database");
                        GenerateData(context);
                        await context.SaveChangesAsync();
                        logger.LogInformation("Successfully seed the database");
                    }                    
                }
            }
            catch (Exception ex) 
            { 
                logger.LogError(ex.Message);
            }
        }

        public static void GenerateData(CaptiveDataContext context) 
        {
            var bankInfos = new List<BankInfo>() {
                new BankInfo
                {
                    Id = Guid.NewGuid(),
                    BankName = "Metrobank",
                    ShortName = "mbtc",                    
                    CreatedDate = DateTime.Now,
                },
                new BankInfo
                {
                    Id = Guid.NewGuid(),
                    BankName = "Security Bank",
                    ShortName = "sbtc",
                    CreatedDate = DateTime.Now,
                },            
            };         

            var bankBranch = new List<BankBranches>() { 
                new BankBranches
                {
                    Id = Guid.Empty,
                    BankInfoId = bankInfos[0].Id,
                    BranchName = "Main Branch",
                    BranchAddress1 = "Sta Cruz",
                    BranchAddress2 = "Quezon City",
                    BranchStatus = BranchStatus.Active,
                    BRSTNCode = "011020011",
                    BankInfo = bankInfos[0],                                       
                },
                new BankBranches
                {
                    Id = Guid.Empty,
                    BankInfoId = bankInfos[0].Id,
                    BranchName = "Valenzuela Branch",
                    BranchAddress1 = "Karuhatan",
                    BranchAddress2 = "Valenzuela City",
                    BranchStatus = BranchStatus.Active,
                    BRSTNCode = "011020012",
                    BankInfo = bankInfos[0],
                },
                new BankBranches
                {
                    Id = Guid.Empty,
                    BankInfoId = bankInfos[1].Id,
                    BranchName = "Main Branch",
                    BranchAddress1 = "36th Ave",
                    BranchAddress2 = "Taguig City",
                    BranchAddress3 = "BGC",
                    BranchStatus = BranchStatus.Active,
                    BRSTNCode = "011020011",
                    BankInfo = bankInfos[1],
                },
            };

            var product = new List<Product>()
            {
                new Product
                {
                    Id = Guid.Empty,
                    ProductName = "ACT Product",
                    ProductSequence = 1,
                    BankInfoId = bankInfos[0].Id,
                    BankInfo = bankInfos[0]
                },
                new Product
                {
                    Id = Guid.Empty,
                    ProductName = "CWS Product",
                    ProductSequence = 2,
                    BankInfoId = bankInfos[0].Id,
                    BankInfo = bankInfos[0]
                }
            };

            var tag = new List<Tag>()
            {
                new Tag
                {
                    Id = Guid.NewGuid(),
                    isDefaultTag = true,
                    SearchByBranch = true,
                    SearchByProduct = true,
                    SearchByFormCheck = true
                }
            };
            var checkInventory = new List<CheckInventory>()
            {
                new CheckInventory
                {
                    Id = Guid.NewGuid(),
                    NumberOfPadding = 5,
                    StartingSeries = 1,
                    SeriesPatern = "ABCD",
                    WarningSeries = 500,
                    isRepeating = false,
                    TagId = tag.First().Id,
                    Tag = tag.First(),
                    IsActive = true,
                }
            };

            var productConfiguration = new List<ProductConfiguration>{ 
                new ProductConfiguration
                {
                    Id = Guid.NewGuid(),
                    ProductId = product[0].Id,
                    isActive = true,
                    ConfigurationData = "{\"hasPassword\":1,\"hasBarcode\":1,\"tableName\":\"ChkBook\",\"columnDefinition\":[{\"fieldName\":\"checkType\",\"columnName\":\"ChkType\"},{\"fieldName\":\"brstn\",\"columnName\":\"RTNO\"},{\"fieldName\":\"accountNumber\",\"columnName\":\"Acctno\"},{\"fieldName\":\"Account\",\"columnName\":\"ChkType\"},{\"fieldName\":\"accountName1\",\"columnName\":\"AcctNm1\"},{\"fieldName\":\"accountName2\",\"columnName\":\"AcctNm2\"},{\"fieldName\":\"concode\",\"columnName\":\"ContCode\"},{\"fieldName\":\"quantity\",\"columnName\":\"OrderQty\"},{\"fieldName\":\"formType\",\"columnName\":\"FormType\"},{\"fieldName\":\"batch\",\"columnName\":\"batch\"}]}",
                    ConfigurationType = ConfigurationType.MdbConfiguration,
                    Product = product[0],
                    FileName = "ACT"
                },
                new ProductConfiguration
                {
                    Id = Guid.NewGuid(),
                    ProductId = product[0].Id,
                    isActive = true,
                    ConfigurationData = "{\"hasPassword\":1,\"hasBarcode\":1,\"tableName\":\"ChkBook\",\"columnDefinition\":[{\"fieldName\":\"checkType\",\"columnName\":\"ChkType\"},{\"fieldName\":\"brstn\",\"columnName\":\"RTNO\"},{\"fieldName\":\"accountNumber\",\"columnName\":\"Acctno\"},{\"fieldName\":\"Account\",\"columnName\":\"ChkType\"},{\"fieldName\":\"accountName1\",\"columnName\":\"AcctNm1\"},{\"fieldName\":\"accountName2\",\"columnName\":\"AcctNm2\"},{\"fieldName\":\"concode\",\"columnName\":\"ContCode\"},{\"fieldName\":\"quantity\",\"columnName\":\"OrderQty\"},{\"fieldName\":\"formType\",\"columnName\":\"FormType\"},{\"fieldName\":\"batch\",\"columnName\":\"batch\"}]}",
                    ConfigurationType = ConfigurationType.MdbConfiguration,
                    Product = product[1],
                    FileName = "CWS"
                }
            };

            var formCheck = new List<FormChecks>
            {
               new FormChecks
               {
                   Id = Guid.NewGuid(),
                   CheckType = "B",
                   FormType = "16",
                   Quantity = 10,
                   Product = product[0],
                   ProductId = product[0].Id,
                   Description = "ACT Form Check Sample",
                   FileInitial = "ACT"
               },
               new FormChecks
               {
                   Id = Guid.NewGuid(),
                   CheckType = "B",
                   FormType = "16",
                   Quantity = 10,
                   Product = product[0],
                   ProductId = product[0].Id,
                   Description = "CWS Form Check Sample",
                   FileInitial = "CWS"
               }
            };

            context.AddRange(bankInfos);
            context.AddRange(bankBranch);
            context.AddRange(checkInventory);
            context.AddRange(product);
            context.AddRange(productConfiguration);           
            context.AddRange(formCheck);
        }
    }


}
