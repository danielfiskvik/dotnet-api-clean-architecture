using Microsoft.AspNetCore.Mvc;
using ModularApp.Modules.Workspace.Application.Common;
using ModularApp.Modules.Workspace.Application.Interfaces;
using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharactersController(
    ICharacterEngine characterEngine,
    ICharacterRepository characterRepository,
    IRepository repository,
    ICharacterMetadataService characterMetadataService,
    IWriteAnkiFileService writeAnkiFileService,
    IWaniKaniToAnkiService waniKaniToAnkiService)
{

    [HttpGet]
    public async Task<IEnumerable<Character>> GetCharactersAsync(CancellationToken ct)
    {
        return await characterRepository.GetCharactersAsync(ct);
    }
    
    [HttpPost("TestUnitOfWorkWithTransactionPattern")]
    public async Task<IQueryable<Character>> CreateCharactersTestAsync(CancellationToken ct)
    {
        await characterEngine.TestCreateCharacterAsync(ct);

        return repository.SecureWithNoTracking<Character>();
    }
    
    [HttpPost("BeginSyncJob")]
    public async Task<IQueryable<Character>> BeginSyncJobAsync(CancellationToken ct)
    {
        await characterMetadataService.BeginSyncJobAsync(ct);

        return repository.SecureWithNoTracking<Character>();
    }
    
    [HttpPost("MakeAnkiDeck")]
    public async Task MakeAnkiDeckAsync(CancellationToken ct)
    {
        await writeAnkiFileService.MakeAnkiDeckAsync(ct);
    }
    
    [HttpPost("MakeAnkiCardFromWaniKaniUrl")]
    public async Task<string> MakeAnkiDeckAsync(string url, CancellationToken ct)
    {
        return await waniKaniToAnkiService.GetAnkiCardFromWaniKaniUrl(url, ct);
    }
    
    [HttpPost("GetRawHtmlFromUrl")]
    public async Task<string> GetRawHtmlFromUrl(string url, CancellationToken ct)
    {
        return await waniKaniToAnkiService.GetRawHtmlFromUrl(url, ct);
    }
}