using Scalar.AspNetCore;
using University.Api.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiDocs();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference().AllowAnonymous();
}

app.MapHealthChecks("/health/live", new() { Predicate = _ => false }).AllowAnonymous();
app.MapHealthChecks("/health/ready", new() { Predicate = c => c.Tags.Contains("ready") }).AllowAnonymous();

app.Run();

public partial class Program;
