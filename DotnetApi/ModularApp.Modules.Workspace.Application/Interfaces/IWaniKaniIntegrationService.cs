namespace ModularApp.Modules.Workspace.Application.Interfaces;

public interface IWaniKaniIntegrationService
{
    Task<string?> GetSearchResultHasHtmlStringAsync(string character, CancellationToken ct);
    
    Task<(string?, bool)> GetHtmlAsStringAsync(string href, CancellationToken ct);
}