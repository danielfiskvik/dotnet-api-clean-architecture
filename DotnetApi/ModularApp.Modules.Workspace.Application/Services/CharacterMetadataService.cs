using System.Reflection;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using ModularApp.Modules.Workspace.Application.Common;
using ModularApp.Modules.Workspace.Application.Helpers;
using ModularApp.Modules.Workspace.Application.Interfaces;
using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.Application.Services;

public class CharacterMetadataService : ICharacterMetadataService
{
    private readonly IWaniKaniIntegrationService _waniKaniIntegrationService;
    private readonly IUnitOfWork _unitOfWork;

        public CharacterMetadataService(
            IWaniKaniIntegrationService waniKaniIntegrationService,
            IUnitOfWork unitOfWork)
        {
            _waniKaniIntegrationService = waniKaniIntegrationService;
            _unitOfWork = unitOfWork;
        }

    public async Task BeginSyncJobAsync(CancellationToken ct)
    {
        // var currentDirectory = Directory.GetCurrentDirectory();
        
        var asm = Assembly.GetExecutingAssembly();
        var currentDirectory = Path.GetDirectoryName(asm.Location) ?? string.Empty;

        // Step 1: Read all kanji's to download from file
        var kanjiFilePath = Path.Combine(currentDirectory, "Assets", "RememberingTheKanjiList.txt");
        var kanjiFileContent = await File.ReadAllTextAsync(kanjiFilePath, ct);

        var kanjiList = kanjiFileContent
            .Replace("\r\n", "")
            .Split(",");
        
        // const string kanji = "一";
        const string source = "WaniKani";

        var anyCharactersToFetch = await _unitOfWork.ApplicationDbContext.CharacterMetadatas
            .AnyAsync(x => x.Source == source && !kanjiList.Contains(x.Characters), ct);
        
        if (!anyCharactersToFetch)
            Console.WriteLine("Hurray!");

        var level = 1;
        foreach (var kanji in kanjiList)
        {
            try
            {
                Console.WriteLine($"Check if kanji '{kanji}' exists.");

                var isMigrated = await _unitOfWork.ApplicationDbContext.CharacterMetadatas
                    .Where(x => x.Characters == kanji && x.Source == source)
                    .AsNoTracking()
                    .Select(x => x.IsMigrated)
                    .FirstOrDefaultAsync(ct);

                if (isMigrated)
                {
                    level++;
                    continue;
                }
                
                if (level % 10 == 0)
                {
                    var random = new Random();
                    var delayInSeconds = random.Next(1, 4); // Generates a random number between 1 and 3 (inclusive)

                    Console.WriteLine("Delaying for " + delayInSeconds + " seconds...");
                
                    Thread.Sleep(delayInSeconds * 1000); // Convert delay to milliseconds

                    Console.WriteLine("Delay complete!");
                }

                // var newData = await FetchDataAsync(kanji, index, source, ct);
                CharacterMetadata? newData;
                try
                {
                    // Step 2. Fetch always "/kanji" first since it is kanji we are searching for (guarantee match)
                    var (kanjiCharacter, hrefOrError1, succeeded1) = await FetchKanjiAndMapToCharacterAsync(
                        kanji, level, 2, ct);
        
                    var (vocabularyCharacter, hrefOrError2, succeeded2) = await FetchVocabularyAndMapToCharacterAsync(
                        kanji, level, 3, ct);
        
                    var (radicalCharacter, error, succeeded3) = await FetchRadicalAndMapToCharacterAsync(
                        kanji, hrefOrError1 ?? hrefOrError2 ?? string.Empty, level, 1, ct);
        
                    // Step 3. Make html and update status
                    var generatedHtml = WaniKaniHtmlToAnkiHtmlHelper.MakeHtml(
                        radicalCharacter: radicalCharacter,
                        kanjiCharacter: kanjiCharacter,
                        vocabularyCharacter: vocabularyCharacter);

                    generatedHtml = generatedHtml.Replace("</br>", "");

                    var status = "";
            
                    status += !succeeded3
                        ? $"Radical: {error}.{Environment.NewLine}"
                        : $"Radical: No error.{Environment.NewLine}";
            
                    status += !succeeded1
                        ? $"Kanji: {hrefOrError1}.{Environment.NewLine}"
                        : $"Kanji: No error.{Environment.NewLine}";
            
                    status += !succeeded2
                        ? $"Vocabulary: {hrefOrError2}.{Environment.NewLine}"
                        : $"Vocabulary: No error.{Environment.NewLine}";

                    if (succeeded3 && radicalCharacter != null)
                        await SaveCharacterAsync(radicalCharacter, ct);
                    
                    if (succeeded1 && kanjiCharacter != null)
                        await SaveCharacterAsync(kanjiCharacter, ct);
                    
                    if (succeeded2 && vocabularyCharacter != null)
                        await SaveCharacterAsync(vocabularyCharacter, ct);
                    
                
                    newData = new CharacterMetadata
                    {
                        Characters = kanji,
                        Source = source,
                        IsMigrated = succeeded1 && succeeded2 && succeeded3, // If all is successful
                        Html = generatedHtml,
                        Status = status
                    };
                    
                    if (!newData.IsMigrated)
                        Console.WriteLine("Not migrated");
                }
                catch (Exception e)
                {
                    newData = new CharacterMetadata
                    {
                        Characters = kanji,
                        Source = source,
                        IsMigrated = false,
                        Html = null,
                        Status = e.Message,
                    };
                }
                
                var characterMetadata = await _unitOfWork.ApplicationDbContext.CharacterMetadatas
                    .Where(x => x.Characters == kanji && x.Source == source)
                    .FirstOrDefaultAsync(ct);
            
                // Post
                if (characterMetadata == null)
                {
                    await _unitOfWork.AddAsync(newData, ct);
                    await _unitOfWork.SaveChangesAsync(ct);

                    level++;
                
                    continue;
                }
            
                // Patch
                characterMetadata.IsMigrated = newData.IsMigrated;
                characterMetadata.Html = newData.Html;
                characterMetadata.Status = newData.Status;
            
                await _unitOfWork.SaveChangesAsync(ct);
            
                level++;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    private async Task SaveCharacterAsync(Character character, CancellationToken ct)
    {
        // TODO Transaction
        var existingCharacter = await _unitOfWork.ApplicationDbContext.Characters
            .Where(x =>
                x.Characters == character.Characters && x.Type == character.Type && x.Source == character.Source)
            .Include(x => x.Meanings)
            .Include(x => x.Readings)
            .Include(x => x.ContextSentences)
            .FirstOrDefaultAsync(ct);

        if (existingCharacter == null)
        {
            await _unitOfWork.AddAsync(character, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            
            return;
        }

        existingCharacter.MeaningMnemonic = character.MeaningMnemonic;
        existingCharacter.MeaningMnemonicHint = character.MeaningMnemonicHint;
        
        existingCharacter.MeaningExplanation = character.MeaningExplanation;
        existingCharacter.MeaningExplanationHint = character.MeaningExplanationHint;
        
        existingCharacter.ReadingMnemonic = character.ReadingMnemonic;
        existingCharacter.ReadingMnemonicHint = character.ReadingMnemonicHint;
        
        existingCharacter.ReadingExplanation = character.ReadingExplanation;
        existingCharacter.ReadingExplanationHint = character.ReadingExplanationHint;
        
        existingCharacter.Level = character.Level;
        existingCharacter.LessonPosition = character.LessonPosition;
        existingCharacter.ExternalUrl = character.ExternalUrl;
        
        existingCharacter.MeaningMnemonicOriginal = character.MeaningMnemonicOriginal;
        existingCharacter.MeaningMnemonicHintOriginal = character.MeaningMnemonicHintOriginal;
        
        existingCharacter.MeaningExplanationOriginal = character.MeaningExplanationOriginal;
        existingCharacter.ReadingMnemonicOriginal = character.ReadingMnemonicOriginal;
        
        existingCharacter.ReadingMnemonicHintOriginal = character.ReadingMnemonicHintOriginal;
        existingCharacter.ReadingExplanationHintOriginal = character.ReadingExplanationHintOriginal;
        
        await _unitOfWork.SaveChangesAsync(ct);
        
        if (character.Meanings.Any())
        {
            _unitOfWork.ApplicationDbContext.Meanings.RemoveRange(existingCharacter.Meanings);
            await _unitOfWork.SaveChangesAsync(ct);

            existingCharacter.Meanings = character.Meanings;

            await _unitOfWork.AddRangeAsync(existingCharacter.Meanings.ToList(), ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
        
        if (character.Readings.Any())
        {
            _unitOfWork.ApplicationDbContext.Readings.RemoveRange(existingCharacter.Readings);
            await _unitOfWork.SaveChangesAsync(ct);

            existingCharacter.Readings = character.Readings;

            await _unitOfWork.AddRangeAsync(existingCharacter.Readings.ToList(), ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
        
        if (character.ContextSentences.Any())
        {
            _unitOfWork.ApplicationDbContext.ContextSentences.RemoveRange(existingCharacter.ContextSentences);
            await _unitOfWork.SaveChangesAsync(ct);

            existingCharacter.ContextSentences = character.ContextSentences;

            await _unitOfWork.AddRangeAsync(existingCharacter.ContextSentences.ToList(), ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }

    private async Task<(Character?, string?, bool)> FetchKanjiAndMapToCharacterAsync(
        string kanji, int level, int lessonPosition, CancellationToken ct)
    {
        // var currentDirectory = Directory.GetCurrentDirectory();
        // var kanjiResponseFilePath = Path.Combine(currentDirectory, "Assets", "KanjiResponse.html");
        // var kanjiResponseFileContent = File.ReadAllText(kanjiResponseFilePath);
        
        var (result, succeeded) = await _waniKaniIntegrationService.GetHtmlAsStringAsync($"/kanji/{kanji}", ct);
        if (!succeeded)
            return (null, result, succeeded);

        try
        {
            #region Fetch, parse, and make object from "/kanji"
            var kanjiHtmlDocument = new HtmlDocument();
            kanjiHtmlDocument.LoadHtml(result);

            var kanjiCharacter = new Character
            {
                Characters = kanji,
                Type = "Kanji",
                Source = "WaniKani",
                ExternalUrl = $"https://www.wanikani.com/kanji/{kanji}",
                Level = level,
                LessonPosition = lessonPosition
            };

            kanjiCharacter.CopyDataFromHtmlDocument(kanjiHtmlDocument);
            kanjiCharacter.ConvertCharacterPropertiesToAnkiHtml();
            #endregion
        
            var nodes = kanjiHtmlDocument.DocumentNode
                .TrySelectNodesFromBaseOrDefault("component-character component-character--radical");

            var radicalHref = nodes
                .Select(x => x.GetAttributeValue("href", ""))
                .FirstOrDefault();

            return (kanjiCharacter, radicalHref, true);
        }
        catch (Exception e)
        {
            return (null, e.Message, false);
        }
    }
    
    private async Task<(Character?, string?, bool)> FetchVocabularyAndMapToCharacterAsync(
        string kanji, int level, int lessonPosition, CancellationToken ct)
    {
        // var currentDirectory = Directory.GetCurrentDirectory();
        // var vocabularyResponseFilePath = Path.Combine(currentDirectory, "Assets", "VocabularyResponse.html");
        // var vocabularyResponseFileContent = File.ReadAllText(vocabularyResponseFilePath);
        
        var (result, succeeded) = await _waniKaniIntegrationService.GetHtmlAsStringAsync($"/vocabulary/{kanji}", ct);
        if (!succeeded)
            return (null, result, succeeded);

        try
        {
            #region Fetch, parse, and make object from "/vocabulary/{kanji}
            var vocabularyHtmlDocument = new HtmlDocument();
            vocabularyHtmlDocument.LoadHtml(result);
            
            var vocabularyCharacter = new Character
            {
                Characters = kanji,
                Type = "Vocabulary",
                Source = "WaniKani",
                ExternalUrl = $"https://www.wanikani.com/vocabulary/{kanji}",
                Level = level,
                LessonPosition = lessonPosition
            };
        
            vocabularyCharacter.CopyDataFromHtmlDocument(vocabularyHtmlDocument);
            vocabularyCharacter.ConvertCharacterPropertiesToAnkiHtml();
            #endregion
        
            var nodes = vocabularyHtmlDocument.DocumentNode
                .TrySelectNodesFromBaseOrDefault("component-character component-character--radical");

            var radicalHref = nodes
                .Select(x => x.GetAttributeValue("href", ""))
                .FirstOrDefault();

            return (vocabularyCharacter, radicalHref, true);
        }
        catch (Exception e)
        {
            return (null, e.Message, false);
        }
    }
    
    private async Task<(Character?, string?, bool)> FetchRadicalAndMapToCharacterAsync(
        string kanji, string href, int level, int lessonPosition, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(href))
            return (null, "No href", true);
        
        // var currentDirectory = Directory.GetCurrentDirectory();
        // var radicalResponseFilePath = Path.Combine(currentDirectory, "Assets", "RadicalResponse.html");
        // var radicalResponseFileContent = File.ReadAllText(radicalResponseFilePath);
        
        var (result, succeeded) = await _waniKaniIntegrationService.GetHtmlAsStringAsync($"{href}", ct);
        if (!succeeded)
            return (null, result, succeeded);

        try
        {
            #region Fetch, parse, and make object from "/radicals/{radicalName}

            var radicalHtmlDocument = new HtmlDocument();
            radicalHtmlDocument.LoadHtml(result);
            
            var radicalCharacter = new Character
            {
                Characters = kanji,
                Type = "Radical",
                Source = "WaniKani",
                ExternalUrl = $"https://www.wanikani.com{href}",
                Level = level,
                LessonPosition = lessonPosition
            };
            
            radicalCharacter.CopyDataFromHtmlDocument(radicalHtmlDocument);
            radicalCharacter.ConvertCharacterPropertiesToAnkiHtml();
            #endregion

            return (radicalCharacter, null, true);
        }
        catch (Exception e)
        {
            return (null, e.Message, false);
        }
    }
}