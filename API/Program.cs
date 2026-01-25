using API;
using Application;
using Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddInfraServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

// Usar o método de configuração de middleware
app.ConfigureApiMiddleware();

app.Run();
