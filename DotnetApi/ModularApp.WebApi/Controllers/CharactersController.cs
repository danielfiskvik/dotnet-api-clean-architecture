using Microsoft.AspNetCore.Mvc;
using ModularApp.Modules.Workspace.Application.Common;
using ModularApp.Modules.Workspace.Application.Interfaces;
using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharactersController
{
    private readonly ICharacterEngine _characterEngine;
    private readonly ICharacterRepository _characterRepository;
    private readonly IRepository _repository;

    public CharactersController(ICharacterEngine characterEngine, ICharacterRepository characterRepository, IRepository repository)
    {
        _characterEngine = characterEngine;
        _characterRepository = characterRepository;
        _repository = repository;
    }
    
    [HttpGet]
    public async Task<IEnumerable<Character>> GetCharactersAsync(CancellationToken ct)
    {
        return await _characterRepository.GetCharactersAsync(ct);
    }
    
    [HttpPost("TestUnitOfWorkWithTransactionPattern")]
    public async Task<IQueryable<Character>> CreateCharactersTestAsync(CancellationToken ct)
    {
        await _characterEngine.TestCreateCharacterAsync(ct);

        return _repository.SecureWithNoTracking<Character>();
    }
}