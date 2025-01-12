
using Captive.Applications.Batch.Hubs;
using Captive.Applications.Orderfiles.Hubs;
using Captive.Commands.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.ConfigureExtensionServices(builder.Configuration);

builder.Logging.ClearProviders().AddConsole();

var app = builder.Build();

await app.MigrateDatabase(app.Logger);
//await app.SeetData(app.Logger, app.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<BatchHub>("/batch");
app.MapHub<OrderFileHub>("/orderfile");

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.Run();

app.Lifetime.ApplicationStarted.Register(() =>
{
    
});
