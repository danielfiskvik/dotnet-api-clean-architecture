using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.Application.Interfaces;

public interface ICharacterRepository
{
    Task<IEnumerable<Character>> GetCharactersAsync(CancellationToken ct);
}