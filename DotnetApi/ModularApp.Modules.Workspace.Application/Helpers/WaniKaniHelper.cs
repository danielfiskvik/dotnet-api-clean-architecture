using HtmlAgilityPack;

namespace ModularApp.Modules.Workspace.Application.Helpers;

public static class WaniKaniHelper
{
    private static readonly Dictionary<string, string>  Styles = new()
    {
        { "radical-highlight", "background-color: rgb(0, 170, 255); color: rgb(255, 255, 255);" },
        { "kanji-highlight", "background-color: rgb(255, 0, 170); color: rgb(255, 255, 255);" },
        { "vocabulary-highlight", "background-color: rgb(170, 0, 255); color: rgb(255, 255, 255);" },
        { "reading-highlight", "background-color: rgb(72, 72, 72); color: rgb(255, 255, 255);" }
    };
    
    private static string GetHtmlWithHighlighting(HtmlNode? node)
    {
        if (node == null)
            return string.Empty;
        
        foreach (var style in Styles)
        {
            var nodes = node.SelectNodes($".//mark[contains(@class, '{style.Key}')]");
            if (nodes == null) continue;
            
            foreach (var markNode in nodes)
            {
                var spanNode = HtmlNode.CreateNode($"<span style='{style.Value}'>{markNode.InnerHtml}</span>");
                markNode.ParentNode.ReplaceChild(spanNode, markNode);
            }
        }

        return node.InnerHtml;
    }

    
    public static string GetMergedHtmlWithHighlighting(this HtmlDocument doc, string xpath)
    {
        var paragraphs = doc.DocumentNode
            .SelectNodes(xpath)
            ?.Select(GetHtmlWithHighlighting);

        return paragraphs == null
            ? string.Empty
            : string.Join("\n", paragraphs); // Join the paragraphs with two line breaks for readability
    }

    public static string GetText(this HtmlDocument doc, string xpath, string defaultText = "")
    {
        var node = doc.DocumentNode.SelectSingleNode(xpath);
        var text = node?.InnerText.Trim() ?? defaultText;
        return text;
    }
    
    public static string GetAlternativeMeaning(this HtmlDocument doc)
    {
        var node = doc.DocumentNode.SelectSingleNode(
            "//section[@id='section-meaning']//h2[contains(text(), 'Alternative')]/following-sibling::p[@class='subject-section__meanings-items']");

        // Extract the word type text
        var reading = node?.InnerText.Trim() ?? string.Empty;

        return reading;
    }
    
    public static string GetOnYomiReading(this HtmlDocument doc)
    {
        // Find the On’yomi reading div
        var node = doc.DocumentNode.SelectSingleNode(
            "//div[contains(@class, 'subject-readings__reading') and .//h3[contains(text(), 'On’yomi')]]");

        // Extract the reading text
        var reading = node?.SelectSingleNode(
            ".//p[@class='subject-readings__reading-items']")?.InnerText.Trim() ?? "No On’yomi found";
    
        return reading;
    }
    
    public static string GetKunYomiReading(this HtmlDocument doc)
    {
        // Find the Kun’yomi reading div
        var node = doc.DocumentNode.SelectSingleNode(
            "//div[contains(@class, 'subject-readings__reading') and .//h3[contains(text(), 'Kun’yomi')]]");

        // Extract the reading text
        var reading = node?.SelectSingleNode(
            ".//p[@class='subject-readings__reading-items']")?.InnerText.Trim() ?? "No Kun’yomi found";
    
        return reading;
    }
    
    public static string GetNanoriReading(this HtmlDocument doc)
    {
        // Find the Nanori reading div
        var node = doc.DocumentNode.SelectSingleNode(
            "//div[contains(@class, 'subject-readings__reading') and .//h3[contains(text(), 'Nanori')]]");

        // Extract the reading text
        var reading = node?.SelectSingleNode(
            ".//p[@class='subject-readings__reading-items']")?.InnerText.Trim() ?? "No Nanori found";
    
        return reading;
    }
    
    public static string GetVocabularyReading(this HtmlDocument doc)
    {
        // Find the reading inside the div with class 'reading-with-audio__reading' and lang='ja'
        return doc.GetText(
            "//section[@id='section-reading']//div[@class='reading-with-audio__reading' and @lang='ja']",
            "No reading found");
    }
    
    public static string GetVocabularyWordType(this HtmlDocument doc)
    {
        // Find the 'Word Type' section by locating the h2 with text 'Word Type'
        return doc.GetText(
            "//section[@id='section-meaning']//h2[contains(text(), 'Word Type')]/following-sibling::p[@class='subject-section__meanings-items']",
            "No word type found");
    }
}