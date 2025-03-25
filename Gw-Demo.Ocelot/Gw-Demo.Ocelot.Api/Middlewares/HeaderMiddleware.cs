using System.Security.Claims;
using Newtonsoft.Json;
using Ocelot.Metadata;
using Ocelot.Middleware;
using Ocelot.PathManipulation;

namespace Gw_Demo.Ocelot.Api.Middlewares;

public class HeaderMiddleware
{
    private readonly HttpClient _httpClient;

    private const string RequireSetCustomHeader = "RequireSetCustomHeader";
    private const string SetHeaderPath = "host.docker.internal:8080/weatherforecasts";

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
            var weatherForecastDto = JsonConvert.DeserializeObject<WeatherForecastDto>(jsonResponse);
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
    public List<WeatherForecastItem> Data { get; set; }
}

public class WeatherForecastItem
{
    public int Id { get; set; }
}