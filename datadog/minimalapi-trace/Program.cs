using Datadog.Trace;
using Datadog.Trace.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Datadog APM
var settings = TracerSettings.FromDefaultSources();
settings.AnalyticsEnabled = true;
Tracer.Configure(settings);

var app = builder.Build();

// Middleware para rastrear requisições HTTP
app.Use(async (context, next) =>
{
    var tracer = Tracer.Instance;
    using (var scope = tracer.StartActive("http.request"))
    {
        var span = scope.Span;

        try
        {
            // Adicionar tags ao span
            span.SetTag("http.method", context.Request.Method);
            span.SetTag("http.url", context.Request.Path);

            await next.Invoke();

            // Adicionar tags de resposta ao span
            span.SetTag("http.status_code", context.Response.StatusCode.ToString());

            // Marcar o span como erro se o status code for 404
            if (context.Response.StatusCode == 404)
            {
                span.SetTag("error", "true");
            }
        }
        catch (Exception ex)
        {
            // Em caso de exceção, marcar o span como erro
            span.SetTag("error", "true");
            span.SetTag("error.message", ex.Message);
            span.SetTag("error.stack", ex.StackTrace);
            throw;
        }
        finally
        {
            // Certificar-se de que o span é finalizado
            span.Finish();
        }
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