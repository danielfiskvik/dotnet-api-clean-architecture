namespace ModularApp.Modules.Workspace.Application.Interfaces;

public interface IWaniKaniToAnkiService
{
    Task<string> GetRawHtmlFromUrl(string url, CancellationToken ct);

    Task<string> GetAnkiCardFromWaniKaniUrl(string url, CancellationToken ct);
}