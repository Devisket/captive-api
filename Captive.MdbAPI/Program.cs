
using Captive.Data;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.MdbProcessor.Processor.DbfGenerator;
using Captive.MdbProcessor.Processor.DbfProcessor;
using Captive.MdbProcessor.Processor.Interfaces;
using Captive.Model.Processing.Configurations;
using Captive.Processing.Processor.MDBFileProcessor;
using Microsoft.EntityFrameworkCore;

namespace Captive.MdbAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<CaptiveDataContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));
            builder.Services.AddScoped<IReadUnitOfWork, ReadUnitOfWork>();
            builder.Services.AddScoped<IWriteUnitOfWork, WriteUnitOfWork>();
            builder.Services.AddScoped<IProcessor<MdbConfiguration>, MDBFileProcessor>();
            builder.Services.AddScoped<IProcessor<DbfConfiguration>, DbfProcessor>();
            builder.Services.AddScoped<IDbfGenerator, DbfGenerator>();
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
