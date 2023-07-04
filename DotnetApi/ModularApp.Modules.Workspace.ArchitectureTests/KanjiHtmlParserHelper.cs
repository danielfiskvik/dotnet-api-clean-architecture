using HtmlAgilityPack;
using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.ArchitectureTests;

public static class KanjiHtmlParserHelper
{
    public static void CopDataFromHtmlDocument(this Character character, HtmlDocument htmlDocument)
    {
        character.SetMeaning(htmlDocument);

        character.SetReading(htmlDocument);

        character.SetContextSentences(htmlDocument);
    }

    private static void SetMeaning(this Character character, HtmlDocument htmlDocument)
    {
        var meaningSectionNode = htmlDocument.DocumentNode
                .TrySelectSingleNodeFromBaseOrDefault("subject-section subject-section--meaning"); // Alternative: id='meaning'

        if (meaningSectionNode == null)
            return;
        
        var meanings = new List<Meaning>();
        
        var wordTypeParentNode = meaningSectionNode
            .TrySelectNodesOrDefault(meaningSectionNode.XPath, "subject-section__meanings-title")
            .FirstOrDefault(x => x.InnerText != null && x.InnerText.StartsWith("Word Type"))
            ?.ParentNode;

        var wordType = wordTypeParentNode
            ?.TrySelectSingleNodeFromThisOrDefault("subject-section__meanings-items")
            ?.InnerText;
        
        var primaryMeaningParentNode = meaningSectionNode
            .TrySelectNodesOrDefault(meaningSectionNode.XPath, "subject-section__meanings-title")
            .FirstOrDefault(x => x.InnerText != null && x.InnerText.StartsWith("Primary"))
            ?.ParentNode;
        
        var primaryMeaningValue = primaryMeaningParentNode
            ?.TrySelectSingleNodeFromThisOrDefault("subject-section__meanings-items")
            ?.InnerText;

        if (!string.IsNullOrEmpty(primaryMeaningValue))
        {
            var primaryMeaning = new Meaning
            {
                Value = primaryMeaningValue,
                IsPrimary = true,
                WordType = wordType
            };
            
            meanings.Add(primaryMeaning);
        }
        
        var alternativeParentNode = meaningSectionNode
            .TrySelectNodesOrDefault(meaningSectionNode.XPath, "subject-section__meanings-title")
            .FirstOrDefault(x => x.InnerText != null && x.InnerText.StartsWith("Alternative"))
            ?.ParentNode;

        var alternativeMeaningValue = alternativeParentNode
            ?.TrySelectSingleNodeFromThisOrDefault("subject-section__meanings-items")
            ?.InnerText;
        
        if (!string.IsNullOrEmpty(alternativeMeaningValue))
        {
            var alternativeMeaning = new Meaning
            {
                Value = alternativeMeaningValue,
                IsPrimary = false,
                WordType = wordType
            };
            
            meanings.Add(alternativeMeaning);
        }
        
        var meaningKeys = new[] { "Primary", "Alternative", "Word Type", "User Synonyms" };

        var remainingMeaningNodes = meaningSectionNode
            .TrySelectNodesOrDefault(meaningSectionNode.XPath, "subject-section__meanings-title")
            .FirstOrDefault(x => x.InnerText != null && x.InnerText.StartsWith("Primary"))
            ?.ParentNode
            ?.ParentNode
            ?.ChildNodes
            ?.WhereNodeIsOnlyHtmlNode()
            .SelectMany(x =>
                x.TrySelectNodesOrDefault(meaningSectionNode.XPath, "subject-section__meanings-title")
            )
            .Where(x => !meaningKeys.Contains(x.InnerText))
            .ToList() ?? new List<HtmlNode>();

        if (remainingMeaningNodes.Any())
        {
            var values = string.Join(",", remainingMeaningNodes.Select(x => x.InnerText));
            Console.WriteLine($"Unhandled meaning nodes. Values: {values}.");
        }
        
        if (meanings.Any())
            character.Meanings = meanings;

        #region Mnemonic + Hint
        var mnemonicNodeChild = meaningSectionNode
            .TrySelectNodesOrDefault(meaningSectionNode.XPath, "subject-section__subtitle")
            .FirstOrDefault(x => x.InnerText != null && x.InnerText.Contains("Mnemonic"));
        
        // ReSharper disable once InvertIf
        if (mnemonicNodeChild != null)
        {
            var meaningMnemonicNode = mnemonicNodeChild.ParentNode
                .TrySelectNodesOrDefault(meaningSectionNode.XPath, "subject-section__text")
                .FirstOrDefault();
            
            var meaningHintNode = mnemonicNodeChild.ParentNode
                .TrySelectNodesOrDefault(meaningSectionNode.XPath, "subject-hint__text")
                .FirstOrDefault();

            character.MeaningMnemonic = meaningMnemonicNode?.InnerHtml; // TODO Convert to Anki?
            character.MeaningMnemonicOriginal = meaningMnemonicNode?.InnerHtml;

            character.MeaningMnemonicHint = meaningHintNode?.InnerHtml; // TODO Convert to Anki?
            character.MeaningMnemonicHintOriginal = meaningHintNode?.InnerHtml;
        }
        #endregion
        
        #region Explanation + Hint
        var explanationNodeChild = meaningSectionNode
            .TrySelectNodesOrDefault(meaningSectionNode.XPath, "subject-section__subtitle")
            .FirstOrDefault(x => x.InnerText != null && x.InnerText.Contains("Explanation"));
        
        // ReSharper disable once InvertIf
        if (explanationNodeChild != null)
        {
            var meaningExplanationNode = explanationNodeChild.ParentNode
                .TrySelectNodesOrDefault(meaningSectionNode.XPath, "subject-section__text")
                .FirstOrDefault();
            
            var meaningExplanationHintNode = explanationNodeChild.ParentNode
                .TrySelectNodesOrDefault(meaningSectionNode.XPath, "subject-hint__text")
                .FirstOrDefault();

            character.MeaningExplanation = meaningExplanationNode?.InnerHtml; // TODO Convert to Anki?
            character.MeaningExplanationOriginal = meaningExplanationNode?.InnerHtml;

            character.MeaningExplanationHint = meaningExplanationHintNode?.InnerHtml; // TODO Convert to Anki?
            character.MeaningExplanationHintOriginal = meaningExplanationHintNode?.InnerHtml;
        }
        #endregion
    }

    private static void SetReading(this Character character, HtmlDocument htmlDocument)
    {
        var readingSectionNode = htmlDocument.DocumentNode
            .TrySelectSingleNodeFromBaseOrDefault("subject-section subject-section--reading"); // Alternative: id='reading'
        
        if (readingSectionNode == null)
            return;

        #region Readings
        var subjectReadingNodes = readingSectionNode
            .TrySelectNodesOrDefault(readingSectionNode.XPath, "subject-readings")
            .FirstOrDefault()
            ?.ChildNodes
            ?.WhereNodeIsOnlyHtmlNode()
            .ToList() ?? new List<HtmlNode>();

        var readings = new List<Reading>();
        
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var subjectReading in subjectReadingNodes)
        {
            var readingTitleNode = subjectReading.ChildNodes
                .GetNodeByAttributeValueFirstOrDefault("subject-readings__reading-title");
            
            if (readingTitleNode == null)
                continue;

            var attributeValue = readingTitleNode.ParentNode
                .GetAttributeValue("class", "") ?? string.Empty;

            var isPrimary = attributeValue.EndsWith("--primary");
            var readingValue = readingTitleNode.InnerText;
            
            var readingType= subjectReading.ChildNodes
                .GetNodeByAttributeValueFirstOrDefault("subject-readings__reading-items")
                ?.InnerText;

            var reading = new Reading
            {
                Type = readingType,
                Value = readingValue,
                IsPrimary = isPrimary
            };
            
            readings.Add(reading);
        }

        if (readings.Any())
            character.Readings = readings;
        #endregion

        #region Reading Mnemonics and Hint
        var readingMnemonicSectionNode = readingSectionNode
            .TrySelectSingleNodeOrDefault(readingSectionNode.XPath, "subject-section__subtitle", "Mnemonic")
            ?.ParentNode;

        // ReSharper disable once InvertIf
        if (readingMnemonicSectionNode != null)
        {
            var readingMnemonicNode = readingMnemonicSectionNode
                .TrySelectNodesOrDefault(readingMnemonicSectionNode.XPath, "subject-section__text")
                .FirstOrDefault();

            var readingHintNode = readingMnemonicSectionNode
                .TrySelectNodesOrDefault(readingMnemonicSectionNode.XPath, "subject-hint__text")
                .FirstOrDefault();
            
            character.ReadingMnemonic = readingMnemonicNode?.InnerHtml; // TODO Convert to Anki?
            character.ReadingMnemonicOriginal = readingMnemonicNode?.InnerHtml;

            character.ReadingMnemonicHint = readingHintNode?.InnerHtml; // TODO Convert to Anki?
            character.ReadingMnemonicHintOriginal = readingHintNode?.InnerHtml;
        }
        #endregion
        
        #region Explanation + Hint
        var explanationNodeChild = readingSectionNode
            .TrySelectNodesOrDefault(readingSectionNode.XPath, "subject-section__subtitle")
            .FirstOrDefault(x => x.InnerText != null && x.InnerText.Contains("Explanation"));
        
        // ReSharper disable once InvertIf
        if (explanationNodeChild != null)
        {
            var meaningExplanationNode = explanationNodeChild.ParentNode
                .TrySelectNodesOrDefault(readingSectionNode.XPath, "subject-section__text")
                .FirstOrDefault();
            
            var meaningExplanationHintNode = explanationNodeChild.ParentNode
                .TrySelectNodesOrDefault(readingSectionNode.XPath, "subject-hint__text")
                .FirstOrDefault();

            character.ReadingExplanation = meaningExplanationNode?.InnerHtml; // TODO Convert to Anki?
            character.ReadingExplanationOriginal = meaningExplanationNode?.InnerHtml;

            character.ReadingExplanationHint = meaningExplanationHintNode?.InnerHtml; // TODO Convert to Anki?
            character.ReadingExplanationHintOriginal = meaningExplanationHintNode?.InnerHtml;
        }
        #endregion
    }
    
    private static void SetContextSentences(this Character character, HtmlDocument htmlDocument)
    {
        var contextSectionNode = htmlDocument.DocumentNode
            .TrySelectSingleNodeFromBaseOrDefault("subject-section subject-section--context"); // Alternative: id='reading'
        
        if (contextSectionNode == null)
            return;

        #region ContextSentences
        var contextSentencesSectionNode = contextSectionNode
            .TrySelectSingleNodeOrDefault(contextSectionNode.XPath, "subject-section__subtitle", "Context Sentences")
            ?.ParentNode;

        var contextSentencesNodes = contextSentencesSectionNode
            ?.TrySelectNodesOrDefault(
                contextSentencesSectionNode.XPath, "subject-section__text subject-section__text--grouped"
            )
            .ToList() ?? new List<HtmlNode>();

        var contextSentences = new List<ContextSentence>();
        
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var contextSentencesNode in contextSentencesNodes)
        {
            var sentenceInJapanese = contextSentencesNode.ChildNodes
                .GetNodeByLangAttributeFirstOrDefault("ja")
                ?.InnerText;

            var sentenceMeaning = contextSentencesNode.ChildNodes
                .WhereNodeIsOnlyHtmlNode()
                .FirstOrDefault(x => !x.HasAttributes)
                ?.InnerText;
            
            if (string.IsNullOrEmpty(sentenceInJapanese))
                continue;
            
            if (string.IsNullOrEmpty(sentenceMeaning))
                continue;

            var contextSentence = new ContextSentence
            {
                Ja = sentenceInJapanese,
                En = sentenceMeaning
            };
            
            contextSentences.Add(contextSentence);
        }

        if (contextSentences.Any())
            character.ContextSentences = contextSentences;

        #endregion

        // TODO Common Word Combinations. See https://www.wanikani.com/vocabulary/兄弟
    }
}