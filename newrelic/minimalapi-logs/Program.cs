using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Enrichers.NewRelic;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Serilog com New Relic
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithNewRelic() // Adiciona o enricher New Relic
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Middleware para rastrear requisições HTTP
app.Use(async (context, next) =>
{
    var logger = Log.ForContext("HttpContext", context);

    // Adiciona informações ao contexto do Serilog
    using (LogContext.PushProperty("RequestPath", context.Request.Path))
    {
        logger.Information("Handling request");
        await next.Invoke();
        logger.Information("Finished handling request with status code {StatusCode}", context.Response.StatusCode);
    }
});

// Mapeamento das rotas
app.MapGet("/", () => "GET no kong");
app.MapGet("/status", () => "GET Status no kong");
app.MapPost("/novo-status", () => "POST Novo Status no Kong");

// Middleware para lidar com 404 - Página não encontrada
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404)
    {
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Not Found");
    }
});

app.Run("http://0.0.0.0:3000");