using Microsoft.EntityFrameworkCore;
using ModularApp.Modules.Workspace.Application.Common;
using ModularApp.Modules.Workspace.Application.Interfaces;
using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.Application.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly IRepository _repository;

    public CharacterRepository(IRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<Character>> GetCharactersAsync(CancellationToken ct)
    {
        return await _repository
            .SecureWithNoTracking<Character>()
            .Include(x => x.Meanings)
            .Include(x => x.Readings)
            .Include(x => x.ContextSentences)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}