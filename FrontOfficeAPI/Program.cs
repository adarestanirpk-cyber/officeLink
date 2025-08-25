using FrontOfficeAPI.Extensions;
using FrontOfficeAPI.Middleware;
using MassTransit;
using Infrastructure.FrontOffice;
using Domain.ValueObjects;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFrontOfficeInfrastructure(builder.Configuration);

builder.Services.AddRabbitMqWithConsumers(builder.Configuration);

builder.Services.Configure<WFCaseDefaults>(builder.Configuration.GetSection("WFCaseDefaults"));

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
