namespace ModularApp.Modules.Workspace.Application.Interfaces;

public interface IWaniKaniIntegrationService
{
    Task<(string?, bool)> GetHtmlAsStringAsync(string href, CancellationToken ct);
}