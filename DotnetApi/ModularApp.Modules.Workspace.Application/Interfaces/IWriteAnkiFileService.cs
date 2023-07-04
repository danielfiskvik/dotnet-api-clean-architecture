namespace ModularApp.Modules.Workspace.Application.Interfaces;

public interface IWriteAnkiFileService
{
    Task MakeAnkiDeckAsync(CancellationToken ct);
}