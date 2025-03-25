using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ocelot.Metadata;
using Ocelot.Middleware;
using Ocelot.PathManipulation;

namespace Gw_Demo.Ocelot.Api.Middlewares;

public class HeaderMiddleware
{
    private readonly HttpClient _httpClient;

    private const string RequireSetCustomHeader = "RequireSetCustomHeader";
    private const string SetHeaderPath = "http://host.docker.internal:8080/weatherforecasts";

    public HeaderMiddleware(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }
    
    public async Task SetHeader(HttpContext context)
    {
        var downstreamRoute = context.Items.DownstreamRoute();
        var requireCustomHeader = downstreamRoute?.GetMetadata<bool>(RequireSetCustomHeader);
        
        if (requireCustomHeader.HasValue && requireCustomHeader.Value)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, SetHeaderPath);
            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
                return;
            
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var weatherForecastDto = JsonSerializer.Deserialize<WeatherForecastDto>(jsonResponse);
            var firstId = weatherForecastDto?.Data.First().Id;
        
            var downStreamRequest = context.Items.DownstreamRequest();
            
            if (firstId is not null)
            {
                downStreamRequest.Headers.Add("X-Id", firstId.ToString());
            }
        }
    }
}

public class WeatherForecastDto
{
    [JsonPropertyName("data")]
    public List<WeatherForecastItem> Data { get; set; }
}

public class WeatherForecastItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}