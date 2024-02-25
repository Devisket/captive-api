
using Captive.Commands.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureExtensionServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.RunDatabaseMigration();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(x=> x.AllowAnyOrigin());

app.Run();

app.Lifetime.ApplicationStarted.Register(() =>
{
    
});
