using System.Text.Json;
using Gw_Demo.Ocelot.Api.Middlewares;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Configuration.AddJsonFile($"ocelot.json", false, true);
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

string ExtractValueFromResponse(string responseContent)
{
    // Implement your logic to extract the value you want from the response
    // This could be parsing JSON, XML, or any other format
        
    // Example for JSON:
    try
    {
        using JsonDocument doc = JsonDocument.Parse(responseContent);
        if (doc.RootElement.TryGetProperty("token", out JsonElement tokenElement))
        {
            return tokenElement.GetString() ?? string.Empty;
        }
    }
    catch { }
        
    return string.Empty;
}
var configuration = new OcelotPipelineConfiguration
{
    AuthorizationMiddleware = async (context, next) =>
    {
        var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
        var headerMiddleware = new HeaderMiddleware(httpClientFactory);
        await headerMiddleware.SetHeader(context);
        await next.Invoke();
    }
};
app.UseOcelot(configuration).Wait();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();