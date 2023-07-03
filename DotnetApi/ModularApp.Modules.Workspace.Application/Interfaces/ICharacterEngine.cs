using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.Application.Interfaces;

public interface ICharacterEngine
{
    Task<List<Character>> TestCreateCharacterAsync(CancellationToken ct);
}