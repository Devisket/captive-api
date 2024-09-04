﻿using Captive.Data.Models;
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
                    ShortName = "MTB",                    
                    CreatedDate = DateTime.Now,
                },
                new BankInfo
                {
                    Id = Guid.NewGuid(),
                    BankName = "Security Bank",
                    BankDescription = "Security Bank sample description",
                    ShortName = "SBTC",
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
                    ProductName = "Sample Product",
                    BankInfoId = bankInfos[0].Id,
                    BankInfo = bankInfos[0]
                }
            };

            var productConfiguration = new List<ProductConfiguration>{ 
                new ProductConfiguration
                {
                    Id = Guid.NewGuid(),
                    ProductId = product[0].Id,
                    isActive = true,
                    ConfigurationData = "",
                    ConfigurationType = ConfigurationType.MsAccessConfiguration,
                    Product = product[0],
                    FileName = "sample"
                }
            };

            context.AddRange(bankInfos);
            context.AddRange(bankBranch);
            context.AddRange(product);
            context.AddRange(productConfiguration);           
        }
    }


}
