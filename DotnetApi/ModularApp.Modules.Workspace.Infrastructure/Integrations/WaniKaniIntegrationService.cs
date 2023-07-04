using System.Text;
using System.Text.Json;
using ModularApp.Modules.Workspace.Application.Interfaces;

namespace ModularApp.Modules.Workspace.Infrastructure.Integrations;

public class WaniKaniIntegrationService : IWaniKaniIntegrationService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WaniKaniIntegrationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string?> GetSearchResultHasHtmlStringAsync(string character, CancellationToken ct)
    {
        var httpClient = _httpClientFactory.CreateClient("WaniKaniHttpClient");
        {
            var httpResponseMessage = await httpClient.GetAsync($"/search?query={character}", ct);

            if (!httpResponseMessage.IsSuccessStatusCode)
                return string.Empty;
            
            await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync(ct);
            
            var result = await JsonSerializer.DeserializeAsync<string>(contentStream, cancellationToken: ct);

            return result;
        }
    }
    
    public async Task<(string?, bool)> GetHtmlAsStringAsync(string href, CancellationToken ct)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("WaniKaniHttpClient");
            {
                var httpResponseMessage = await httpClient.GetAsync($"{href}", ct);

                if (!httpResponseMessage.IsSuccessStatusCode)
                    return (httpResponseMessage.ReasonPhrase, false);
            
                await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync(ct);

                var result = StreamToString(contentStream);// await JsonSerializer.DeserializeAsync<string>(contentStream, cancellationToken: ct);

                return (result, true);
            }
        }
        catch (Exception e)
        {
            return (e.Message, false);
        }
    }
    
    public static string StreamToString(Stream stream)
    {
        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        
        return reader.ReadToEnd();
    }
}