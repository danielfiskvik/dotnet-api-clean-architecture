using System.Text;
using HtmlAgilityPack;
using ModularApp.Modules.Workspace.Application.Helpers;
using ModularApp.Modules.Workspace.Application.Interfaces;

namespace ModularApp.Modules.Workspace.Application.Services;

public class WaniKaniToAnkiService(IWaniKaniIntegrationService waniKaniIntegrationService) : IWaniKaniToAnkiService
{
    public async Task<string> GetRawHtmlFromUrl(string url, CancellationToken ct)
    {
        var (html, succeeded) = await waniKaniIntegrationService.GetHtmlAsStringAsync(url, ct);

        return succeeded ? html ?? string.Empty : string.Empty;
    }
    
    public async Task<string> GetAnkiCardFromWaniKaniUrl(string url, CancellationToken ct)
    {
        var (result, _) = await waniKaniIntegrationService.GetHtmlAsStringAsync(url, ct);
        if (result == null)
            return string.Empty;

        if (url.Contains("/radicals/"))
            return MakeRadicalAnkiCard(result);

        if (url.Contains("/kanji/"))
            return MakeKanjiAnkiCard(result);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (url.Contains("/vocabulary/"))
            return MakeVocabularyAnkiCard(result);

        return string.Empty;
    }

    private static string MakeRadicalAnkiCard(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        
        // Extract Radical Character
        var radicalCharacter = doc.GetText(
                "//span[contains(@class, 'page-header__icon--radical')]",
                "No Kanji found");
        
        // Extract English Meaning
        var englishMeaning = doc.GetText(
            "//span[contains(@class, 'page-header__title-text')]");
        
        // Extract Mnemonic
        var mnemonic = doc.GetMergedHtmlWithHighlighting(
            "//section[@id='section-meaning']//p[@class='subject-section__text']");
        
        var sb = new StringBuilder();
        sb.AppendLine("<br>");
        sb.AppendLine("<div><b>Wani Kani: Radicals</b></div>");
        sb.AppendLine("<div><br></div>");
        sb.AppendLine("<div>");
        sb.AppendLine("    <div>");
        sb.AppendLine($"        <span style=\"background-color: rgb(0, 170, 255); color: rgb(255, 255, 255);\">&nbsp;{radicalCharacter}&nbsp;</span><b>&nbsp;{englishMeaning}</b>");
        sb.AppendLine("    </div>");
        sb.AppendLine("    <div>");
        sb.AppendLine("        <div>Name:</div>");
        sb.AppendLine($"        <div>Primary: {englishMeaning}</div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("    <div>");
        sb.AppendLine("        <div><br></div>");
        sb.AppendLine("        <div>Mnemonic:</div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("    <div>");
        sb.AppendLine($"        {mnemonic}");
        sb.AppendLine("    </div>");
        sb.AppendLine("</div>");
        
        return sb.ToString();
    }
    
    private static string MakeKanjiAnkiCard(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        
        // Extract Radical Character
        var kanjiCharacter = doc.GetText(
            "//span[contains(@class, 'page-header__icon--kanji')]");
        
        // Extract English Meaning
        var englishMeaning = doc.GetText(
            "//span[contains(@class, 'page-header__title-text')]");
        
        // Extract Alternative Meaning
        var alternativeMeaning = doc.GetAlternativeMeaning();
        
        // Extract Meaning Mnemonic
        var meaningMnemonicHtml = doc.GetMergedHtmlWithHighlighting(
                "//section[@id='section-meaning']//p[contains(@class, 'subject-section__text')]");
        
        // Extract Meaning Mnemonic Hint
        var meaningMnemonicHintHtml = doc.GetMergedHtmlWithHighlighting(
                "//section[@id='section-meaning']//p[contains(@class, 'subject-hint__text')]");
        
        // Extract Readings Mnemonic
        var readingsMnemonicHtml = doc.GetMergedHtmlWithHighlighting(
                "//section[@id='section-reading']//p[contains(@class, 'subject-section__text')]");
        
        // Extract Readings Mnemonic Hint
        var readingsMnemonicHintHtml = doc.GetMergedHtmlWithHighlighting(
                "//section[@id='section-reading']//p[contains(@class, 'subject-hint__text')]");
        
        // Extract Kun'yomi Reading
        var onYomiReadingText = doc.GetOnYomiReading();
        
        // Extract Kun'yomi Reading
        var kunYomiReadingText = doc.GetKunYomiReading();
        
        // Extract Nanori Reading
        var nanoriReadingText = doc.GetNanoriReading();

        var sb = new StringBuilder();
        sb.AppendLine("<br>");
        sb.AppendLine("<div><b>Wani Kani: Kanji</b></div>");
        sb.AppendLine("<div><br></div>");
        sb.AppendLine("<div>");
        sb.AppendLine("    <div>");
        sb.AppendLine($"        <span style=\"background-color: rgb(255, 0, 170); color: rgb(255, 255, 255);\">&nbsp;{kanjiCharacter}&nbsp;</span><b>&nbsp;{englishMeaning}</b>");
        sb.AppendLine("    </div>");
        sb.AppendLine("    <div><span>Radical Combination:</span></div>");
        sb.AppendLine("    <div>(INSERT RADICALS HERE)</div>");
        sb.AppendLine("    <br>");
        sb.AppendLine("    <div>Meaning:</div>");
        sb.AppendLine($"    <div>Primary: {englishMeaning}</div>");
        if (!string.IsNullOrWhiteSpace(alternativeMeaning))
        {
            sb.AppendLine($"    <div>Alternative: {alternativeMeaning}</div>");
        }
        sb.AppendLine("    <br>");
        sb.AppendLine("    <div>Meaning Mnemonic:</div>");
        sb.AppendLine($"    <div>{meaningMnemonicHtml}</div>");
        sb.AppendLine("    <br>");
        sb.AppendLine("    <div>Hints:</div>");
        sb.AppendLine($"    <div>{meaningMnemonicHintHtml}</div>");
        sb.AppendLine("    <br>");
        sb.AppendLine("    <div>Readings:</div>");
        sb.AppendLine($"    <div>On’yomi: {onYomiReadingText}</div>");
        sb.AppendLine($"    <div>Kun’yomi: {kunYomiReadingText}</div>");
        sb.AppendLine($"    <div>Nanori: {nanoriReadingText}</div>");
        sb.AppendLine("    <br>");
        sb.AppendLine("    <div>Readings Mnemonic:</div>");
        sb.AppendLine($"    <div>{readingsMnemonicHtml}</div>");
        sb.AppendLine("    <br>");
        sb.AppendLine("    <div>Hints:</div>");
        sb.AppendLine($"    <div>{readingsMnemonicHintHtml}</div>");
        sb.AppendLine("</div>");
        
        return sb.ToString();
    }
    
    private static string MakeVocabularyAnkiCard(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        
        // Extract Radical Character
        var kanjiCharacter = doc.GetText(
            "//span[contains(@class, 'page-header__icon--vocabulary')]");
        
        // Extract English Meaning
        var englishMeaning = doc.GetText(
            "//span[contains(@class, 'page-header__title-text')]");
        
        // Extract Word Type
        var wordType = doc.GetVocabularyWordType();
        
        // Extract Alternative Meaning
        var alternativeMeaning = doc.GetAlternativeMeaning();
        
        // Extract Meaning Mnemonic
        var meaningExplanationHtml = doc.GetMergedHtmlWithHighlighting(
            "//section[@id='section-meaning']//p[contains(@class, 'subject-section__text')]");
        
        // Extract Meaning Explanation
        var meaningExplanationHintHtml = doc.GetMergedHtmlWithHighlighting(
            "//section[@id='section-meaning']//p[contains(@class, 'subject-hint__text')]");
        
        // Extract Readings Mnemonic
        var reading = doc.GetVocabularyReading();
        
        // Extract Reading Explanation
        var readingsExplanationHtml = doc.GetMergedHtmlWithHighlighting(
            "//section[@id='section-reading']//p[@class='subject-section__text']");
        
        // Extract Readings Mnemonic Hint
        var readingsMnemonicHintHtml = doc.GetMergedHtmlWithHighlighting(
            "//section[@id='section-reading']//p[contains(@class, 'subject-hint__text')]");

        var sb = new StringBuilder();
        sb.AppendLine("<br>");
        sb.AppendLine("<div><b>Wani Kani: Vocabulary</b></div>");
        sb.AppendLine("<div><br></div>");
        sb.AppendLine("<div>");
        sb.AppendLine("    <div>");
        sb.AppendLine($"        <span style=\"background-color: rgb(170, 0, 255); color: rgb(255, 255, 255);\">&nbsp;{kanjiCharacter}&nbsp;</span><b>&nbsp;{englishMeaning}</b>");
        sb.AppendLine("    </div>");
        sb.AppendLine("    <div>Meaning:</div>");
        sb.AppendLine($"    <div>Primary: {englishMeaning}</div>");
        if (!string.IsNullOrWhiteSpace(alternativeMeaning))
        {
            sb.AppendLine($"    <div>Alternative: {alternativeMeaning}</div>");
        }
        if (!string.IsNullOrWhiteSpace(wordType))
        {
            sb.AppendLine($"    <div>Word Type: {wordType}</div>");
            sb.AppendLine("    <br>");
        }
        sb.AppendLine("    <div>Meaning Explanation:</div>");
        sb.AppendLine($"    <div>{meaningExplanationHtml}</div>");
        if (!string.IsNullOrEmpty(meaningExplanationHintHtml)) {
            sb.AppendLine("    <br>");
            sb.AppendLine("    <div>Hints:</div>");
            sb.AppendLine($"    <div>{meaningExplanationHintHtml}</div>");
        }
        sb.AppendLine("    <br>");
        sb.AppendLine("    <div>Readings:</div>");
        sb.AppendLine($"    <div>{reading}</div>");
        if (!string.IsNullOrWhiteSpace(readingsExplanationHtml))
        {
            sb.AppendLine("    <br>");
            sb.AppendLine("    <div>Readings Explanation:</div>");
            sb.AppendLine($"    <div>{readingsExplanationHtml}</div>");
        }
        if (!string.IsNullOrEmpty(readingsMnemonicHintHtml))
        {
            sb.AppendLine("    <br>");
            sb.AppendLine("    <div>Hints:</div>");
            sb.AppendLine($"    <div>{readingsMnemonicHintHtml}</div>");
        }
        sb.AppendLine("</div>");
        
        return sb.ToString();
    }
}