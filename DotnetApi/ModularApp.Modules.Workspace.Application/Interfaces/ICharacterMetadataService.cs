namespace ModularApp.Modules.Workspace.Application.Interfaces;

public interface ICharacterMetadataService
{
    Task BeginSyncJobAsync(CancellationToken ct);
}