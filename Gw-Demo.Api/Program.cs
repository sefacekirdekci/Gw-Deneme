using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecasts", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    index,
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();

        return Results.Ok(new { data = forecast });  // JSON nesnesine sarılıyor
    })
    .WithName("GetWeatherForecasts");

app.MapGet("/weatherforecast", (HttpContext context) =>
    {
        var headers = context.Request.Headers;
        var forecastId = headers.TryGetValue("X-Id", out var ids) ? int.Parse(ids.FirstOrDefault()) : 0;
        var forecasts = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    index,
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();

        var forecast = forecasts[forecastId];

        return Results.Ok(new { data = forecast });  // JSON nesnesine sarılıyor
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(int Id, DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
