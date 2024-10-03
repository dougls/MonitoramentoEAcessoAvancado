WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.MapGet("/", () => "GET no kong");

app.MapGet("/status", () => "GET Status no kong");

app.MapPost("/novo-status", () => "POST Novo Status no Kong");

app.Run("http://0.0.0.0:3000");
