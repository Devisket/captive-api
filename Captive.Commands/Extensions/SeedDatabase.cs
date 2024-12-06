using Captive.Data.Models;
using Captive.Data;
using Microsoft.AspNetCore.Builder;
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
                    BankDescription = "Metrobank sample description",
                    ShortName = "mbtc",                    
                    CreatedDate = DateTime.Now,
                },
                new BankInfo
                {
                    Id = Guid.NewGuid(),
                    BankName = "Security Bank",
                    BankDescription = "Security Bank sample description",
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
                    BankInfoId = bankInfos[0].Id,
                    BankInfo = bankInfos[0]
                },
                new Product
                {
                    Id = Guid.Empty,
                    ProductName = "CWS Product",
                    BankInfoId = bankInfos[0].Id,
                    BankInfo = bankInfos[0]
                }
            };

            var checkValidation = new List<CheckValidation>()
            {
                new CheckValidation
                {
                    Id = Guid.Empty,
                    Name = "Check validation by Product",
                    BankInfoId = bankInfos[0].Id,
                    ValidationType = ValidationType.Product
                },
                new CheckValidation
                {
                    Id = Guid.Empty,
                    Name = "Check validation by BRSTN",
                    BankInfoId = bankInfos[0].Id,
                    ValidationType = ValidationType.Branch
                }
            };

            var checkInventory = new List<CheckInventory>()
            {
                new CheckInventory
                {
                    Id = Guid.NewGuid(),
                    CheckValidationId = checkValidation[0].Id,
                    CheckValidation = checkValidation[0],
                    SeriesPatern = "ACT00000"
                },
                new CheckInventory
                {
                    Id = Guid.NewGuid(),
                    CheckValidationId = checkValidation[1].Id,
                    CheckValidation = checkValidation[1],
                    SeriesPatern = "CWS000000"
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
                    CheckValidationId = checkValidation[0].Id,
                    CheckValidation = checkValidation[0],
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
                    CheckValidationId = checkValidation[1].Id,
                    CheckValidation = checkValidation[1],
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
            context.AddRange(checkValidation);
            context.AddRange(checkInventory);
            context.AddRange(product);
            context.AddRange(productConfiguration);           
            context.AddRange(formCheck);
        }
    }


}
