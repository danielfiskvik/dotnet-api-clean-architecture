using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.ArchitectureTests;

public static class WaniKaniHtmlToAnkiHtmlHelper
{
    public static void ConvertCharacterPropertiesToAnkiHtml(this Character character)
    {
        character.MeaningMnemonic = character.MeaningMnemonic.ConvertToAnkiHtml();
        character.MeaningMnemonicHint = character.MeaningMnemonicHint.ConvertToAnkiHtml();
        
        character.MeaningExplanation = character.MeaningExplanation.ConvertToAnkiHtml();
        character.MeaningExplanationHint = character.MeaningExplanationHint.ConvertToAnkiHtml();
        
        character.ReadingMnemonic = character.ReadingMnemonic.ConvertToAnkiHtml();
        character.ReadingMnemonicHint = character.ReadingMnemonicHint.ConvertToAnkiHtml();
        
        character.ReadingExplanation = character.ReadingExplanation.ConvertToAnkiHtml();
        character.ReadingExplanationHint = character.ReadingExplanationHint.ConvertToAnkiHtml();
        character.MeaningMnemonicHint = character.MeaningMnemonicHint.ConvertToAnkiHtml();
    }

    private static string? ConvertToAnkiHtml(this string? value)
    {
        if (value == null)
            return null;

        var valueWithoutWhitespace = value.Replace(" ", "");

        if (!valueWithoutWhitespace.StartsWith("<"))
            value = $"<div>{value}</div>";

        value = value.Replace(
            HtmlConstants.ClassHighlightRadical, 
            $"{HtmlConstants.ClassHighlightRadical} {HtmlConstants.StyleHighlightRadical}"
        );
        
        value = value.Replace(
            HtmlConstants.ClassHighlightKanji, 
            $"{HtmlConstants.ClassHighlightKanji} {HtmlConstants.StyleHighlightKanji}"
        );
        
        value = value.Replace(
            HtmlConstants.ClassHighlightVocabulary, 
            $"{HtmlConstants.ClassHighlightVocabulary} {HtmlConstants.StyleHighlightVocabulary}"
        );
        
        value = value.Replace(
            HtmlConstants.ClassHighlightReading, 
            $"{HtmlConstants.ClassHighlightReading} {HtmlConstants.StyleHighlightReading}"
        );

        return value;
    }

    public static string MakeHtml(
        Character? radicalCharacter,
        Character? kanjiCharacter,
        Character? vocabularyCharacter)
    {
        /*
         <div>
            <h1>Header 1</h1>
            <h2>Header 2</h2>
            <h3>Header 3</h3>
            <h4>Header 4</h4>
            <h5>Header 5</h5>
            <h6>Header 6</h6>
        </div>
         */
        
        var template = kanjiCharacter ?? radicalCharacter ?? vocabularyCharacter;
        if (template == null)
            return string.Empty;
        
        var mainHtml = $"<h1>{template.Source}</h1>";

        if (radicalCharacter != null)
        {
            var subHtml = radicalCharacter.MakeCharacterHtml();
            mainHtml += $"{subHtml}</br>";
        }
        
        if (kanjiCharacter != null)
        {
            var subHtml = kanjiCharacter.MakeCharacterHtml();
            mainHtml += $"{subHtml}</br>";
        }
        
        // ReSharper disable once InvertIf
        if (vocabularyCharacter != null)
        {
            var subHtml = vocabularyCharacter.MakeCharacterHtml();
            mainHtml += $"{subHtml}</br>";
        }

        return mainHtml;
    }

    private static string MakeCharacterHtml(this Character character)
    {
        var mainHtml = $"<h2>{character.Type} {character.Characters}, Level: {character.Level}, Position: {character.LessonPosition}</h2></br>";
        // TODO external url

        mainHtml += character.MakeMeaningSectionHtml();
        mainHtml += character.MakeReadingSectionHtml();
        mainHtml += character.MakeContextSentencesSectionHtml();

        switch (character.Type)
        {
            case "Radical":
                // Todo
                
                // Context Explanation or Mnemonic
                break;
            case "Kanji":
                // Todo
                break;
            case "Vocabulary":
                // Todo
                break;
        }

        return mainHtml;
    }

    private static string MakeMeaningSectionHtml(this Character character)
    {
        var meaningNames = "";
        var meaningMnemonic = "";
        var meaningSection = $"<h3>Meaning</h3></br><hr></br>";
        
        #region Meaning Name
        if (character.Meanings.Any())
        {
            var meanings = character.Meanings
                .Select(meaning => !string.IsNullOrEmpty(meaning.Type)
                    ? $"<div>{meaning.Type}: {meaning.Value}</div>"
                    : $"<div>{meaning.Value}</div>"
                )
                .ToList();

            meaningNames += string.Join("</br>", meanings);
            meaningNames += "</br>";

            if (character.Meanings.Any(x => !string.IsNullOrEmpty(x.WordType)))
            {
                var wordType = character.Meanings
                    .Select(x => x.WordType)
                    .FirstOrDefault();
                        
                meaningNames += $"<div>Word Type: {wordType}</div></br>";
            }
        }
        #endregion

        #region Meaning Explanation / Mnemonic
        var meaning = character.MeaningExplanation ?? character.MeaningMnemonic;
        
        if (!string.IsNullOrEmpty(meaning))
        {
            var meaningTitle = !string.IsNullOrEmpty(character.MeaningMnemonic)
                ? "Mnemonic"
                : "Explanation";
        
            meaningMnemonic += $"<h4>{meaningTitle}</h4></br>";
            meaningMnemonic += $"{meaning}</br>";
            
            var hint = character.MeaningExplanationHint ?? character.MeaningMnemonicHint;

            if (!string.IsNullOrEmpty(hint))
            {
                meaningMnemonic += $"<h4>Hint</h4></br>";
                meaningMnemonic += $"{hint}</br>";
            }
        }
        #endregion
        
        if (string.IsNullOrEmpty(meaningNames) && string.IsNullOrEmpty(meaningMnemonic))
            return string.Empty;
        
        meaningSection += meaningNames;
        meaningSection += meaningMnemonic;

        return meaningSection;
    }
    
    private static string MakeReadingSectionHtml(this Character character)
    {
        var readings = "";
        var readingMnemonic = "";
        var readingSection = $"<h3>Readings</h3></br><hr></br>";
        
        #region Readings
        if (character.Readings.Any())
        {
            var readingItems = character.Readings
                .Select(x => !string.IsNullOrEmpty(x.Type)
                    ? $"<div>{x.Type}: {x.Value} (Primary: {x.IsPrimary})</div>"
                    : $"<div>{x.Value} (Primary: {x.IsPrimary})</div>"
                )
                .ToList();

            readings += string.Join("</br>", readingItems);
            readings += "</br>";
        }
        #endregion

        #region Readings Explanation / Mnemonic
        var reading = character.ReadingExplanation ?? character.ReadingMnemonic;
        
        if (!string.IsNullOrEmpty(reading))
        {
            var readingTitle = !string.IsNullOrEmpty(character.ReadingMnemonic)
                ? "Mnemonic"
                : "Explanation";
        
            readingMnemonic += $"<h4>{readingTitle}</h4></br>";
            readingMnemonic += $"{reading}</br>";
            
            var hint = character.ReadingExplanationHint ?? character.ReadingMnemonicHint;

            if (!string.IsNullOrEmpty(hint))
            {
                readingMnemonic += $"<h4>Hint</h4></br>";
                readingMnemonic += $"{hint}</br>";
            }
        }
        #endregion
        
        if (string.IsNullOrEmpty(readings) && string.IsNullOrEmpty(readingMnemonic))
            return string.Empty;
        
        readingSection += readings;
        readingSection += readingMnemonic;

        return readingSection;
    }
    
    private static string MakeContextSentencesSectionHtml(this Character character)
    {
        var contextSentences = "";
        var readingSection = $"<h3>Context Sentences</h3></br><hr></br>";
        
        #region ContextSentences
        if (character.ContextSentences.Any())
        {
            var contextSentenceList = character.ContextSentences
                .Select(x => $"<div>Japanese: {x.Ja}</div></br><div>English: {x.En}</div>"
                )
                .ToList();

            contextSentences += string.Join("</br></br>", contextSentenceList);
            contextSentences += "</br>";
        }
        #endregion
        
        if (string.IsNullOrEmpty(contextSentences))
            return string.Empty;
        
        readingSection += contextSentences;

        return readingSection;
    }
}

public static class HtmlConstants
{
    // Color
    public const string LightColor = "color: rgb(255, 255, 255)";
    public const string RadicalColor = "background-color: rgb(0, 170, 255)";
    public const string KanjiColor = "background-color: rgb(255, 0, 170)";
    public const string VocabularyColor = "background-color: rgb(170, 0, 255)";
    public const string ReadingColor = "background-color: rgb(85, 85, 85)";
    
    // Highlight
    public const string HighlightRadical = "radical-highlight";
    public const string HighlightKanji = "kanji-highlight";
    public const string HighlightVocabulary = "vocabulary-highlight";
    public const string HighlightReading = "reading-highlight";

    // Class
    public const string ClassHighlightRadical = $"class=\"{HighlightRadical}\"";
    public const string ClassHighlightKanji = $"class=\"{HighlightKanji}\"";
    public const string ClassHighlightVocabulary = $"class=\"{HighlightVocabulary}\"";
    public const string ClassHighlightReading = $"class=\"{HighlightReading}\"";

    // Style
    public const string StyleHighlightRadical = $"style=\"{RadicalColor};{LightColor};\"";
    public const string StyleHighlightKanji = $"style=\"{KanjiColor};{LightColor};\"";
    public const string StyleHighlightVocabulary = $"style=\"{VocabularyColor};{LightColor};\"";
    public const string StyleHighlightReading = $"style=\"{ReadingColor};{LightColor};\"";
}