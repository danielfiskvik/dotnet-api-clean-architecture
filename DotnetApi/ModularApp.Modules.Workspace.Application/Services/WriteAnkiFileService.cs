using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ModularApp.Modules.Workspace.Application.Common;
using ModularApp.Modules.Workspace.Application.Interfaces;

namespace ModularApp.Modules.Workspace.Application.Services;

public class WriteAnkiFileService : IWriteAnkiFileService
{
    private readonly IUnitOfWork _unitOfWork;
    private const string Source = "WaniKani";

    public WriteAnkiFileService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task MakeAnkiDeckAsync(CancellationToken ct)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var currentDirectory = Path.GetDirectoryName(assembly.Location) ?? string.Empty;

        // Step 1: Read all kanji's to download from file
        var kanjiFilePath = Path.Combine(currentDirectory, "Assets", "RememberingTheKanjiList.txt");
        var kanjiFileContent = await File.ReadAllTextAsync(kanjiFilePath, ct);

        var kanjiList = kanjiFileContent
            .Replace("\r\n", "")
            .Split(",");

        var lines = new List<string>
        {
            "#separator:tab",
            "#html:true"
        };

        var level = 1;
        foreach (var kanji in kanjiList)
        {
            var character = await _unitOfWork.ApplicationDbContext.CharacterMetadatas
                .Where(x => x.Characters == kanji && x.Source == Source)
                .AsNoTracking()
                .Select(x => new
                {
                    x.Characters,
                    x.Html
                })
                .FirstOrDefaultAsync(ct);

            level++;
            
            if (character == null)
                continue;
            
            lines.Add($"{character.Characters}\t{character.Html}");
            
            // ReSharper disable once InvertIf
            if (level % 100 == 0)
            {
                var random = new Random();
                var delayInSeconds = random.Next(1, 4); // Generates a random number between 1 and 3 (inclusive)

                Console.WriteLine("Delaying for " + delayInSeconds + " seconds...");
                
                Thread.Sleep(delayInSeconds * 1000); // Convert delay to milliseconds

                Console.WriteLine("Delay complete!");
            }
        }
        
        const string path = @"F:\RememberingTheKanji1Deck.txt";
        await File.WriteAllLinesAsync(path, lines, ct);
    }
}