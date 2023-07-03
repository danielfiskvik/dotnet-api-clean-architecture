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
}