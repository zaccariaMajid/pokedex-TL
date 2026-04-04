using Pokedex.Application;
using Pokedex.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();
