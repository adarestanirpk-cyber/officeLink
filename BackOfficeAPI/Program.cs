using Application.Interfaces;
using Application.Services;
using BackOfficeAPI.Extensions;
using BackOfficeAPI.Middleware;
using Infrastructure.BackOffice;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//add message service
builder.Services.AddScoped<IWFCaseLinkService,WFCaseLinkService>();

builder.Services.AddRabbitMqWithConsumers(builder.Configuration);

builder.Services.AddBackOfficeInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCorrelationId();
app.UseRequestResponseLogging();
app.UseExceptionHandling();

app.UseAuthorization();

app.MapControllers();

app.Run();
