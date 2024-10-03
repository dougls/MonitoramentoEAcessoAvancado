using Datadog.Trace;
using Datadog.Trace.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Datadog.Logs;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Adiciona os serviços de autorização
builder.Services.AddAuthorization();

// Configuração do Datadog APM
var settings = TracerSettings.FromDefaultSources();
settings.AnalyticsEnabled = true;
Tracer.Configure(settings);

var app = builder.Build();

// Middleware para rastrear e logar requisições HTTP
app.Use(async (context, next) =>
{
    var tracer = Tracer.Instance;
    using (var scope = tracer.StartActive("http.request"))
    {
        var span = scope.Span;
        var requestInfo = $"Handling request: {context.Request.Method} {context.Request.Path}";
        Log.Information(requestInfo);
        span.SetTag("http.method", context.Request.Method);
        span.SetTag("http.url", context.Request.Path);

        try
        {
            await next.Invoke();

            span.SetTag("http.status_code", context.Response.StatusCode.ToString());

            if (context.Response.StatusCode == 404)
            {
                span.SetTag("error", "true");
                Log.Error("404 Not Found: {Method} {Path}", context.Request.Method, context.Request.Path);
            }
        }
        catch (Exception ex)
        {
            span.SetTag("error", "true");
            span.SetTag("error.message", ex.Message);
            span.SetTag("error.stack", ex.StackTrace);
            Log.Error(ex, "Unhandled exception while processing request: {Method} {Path}", context.Request.Method, context.Request.Path);
            throw;
        }
        finally
        {
            span.Finish();
            var responseInfo = $"Finished handling request: {context.Request.Method} {context.Request.Path} with status code {context.Response.StatusCode}";
            Log.Information(responseInfo);
        }
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

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