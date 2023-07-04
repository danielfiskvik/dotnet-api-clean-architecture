using HtmlAgilityPack;

namespace ModularApp.Modules.Workspace.ArchitectureTests;

public static class HtmlNodeHelper
{
    public static HtmlNode? GetNodeByAttributeValueFirstOrDefault(
        this HtmlNodeCollection childNodes, string classKey)
    {
        return childNodes
            .WhereNodeIsOnlyHtmlNode()
            .FirstOrDefault(n => n.GetAttributeValue("class", "") == classKey);
    }
    
    public static HtmlNode? GetNodeByLangAttributeFirstOrDefault(
        this HtmlNodeCollection childNodes, string key)
    {
        return childNodes
            .WhereNodeIsOnlyHtmlNode()
            .FirstOrDefault(n => n.GetAttributeValue("lang", "") == key);
    }
    
    public static HtmlNode? GetNodeByAttributeValueFirstOrDefault(
        this IEnumerable<HtmlNode> childNodes, string classKey)
    {
        return childNodes
            .WhereNodeIsOnlyHtmlNode()
            .FirstOrDefault(n => n.GetAttributeValue("class", "") == classKey);
    }
    
    public static IEnumerable<HtmlNode> WhereNodeIsOnlyHtmlNode(
        this HtmlNodeCollection childNodes)
    {
        return childNodes.Where(x => x.NodeType == HtmlNodeType.Element); // or .Name != "#text"
    }
    
    public static IEnumerable<HtmlNode> WhereNodeIsOnlyHtmlNode(
        this IEnumerable<HtmlNode> childNodes)
    {
        return childNodes.Where(x => x.NodeType == HtmlNodeType.Element); // or .Name != "#text"
    }

    public static IEnumerable<HtmlNode> TrySelectNodesOrDefault(this HtmlNode htmlNode, string xpath, string classKey)
    {
        try
        {
            return htmlNode
                .SelectNodes($"{xpath}//*[contains(@class, '{classKey}')]")
                .WhereNodeIsOnlyHtmlNode();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return Array.Empty<HtmlNode>();
        }
    }
    
    public static IEnumerable<HtmlNode> TrySelectNodesFromBaseOrDefault(this HtmlNode htmlNode, string classKey)
    {
        try
        {
            return htmlNode
                .SelectNodes($"//*[@class='{classKey}']")
                .WhereNodeIsOnlyHtmlNode();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return Array.Empty<HtmlNode>();
        }
    }
    
    public static HtmlNode? TrySelectSingleNodeFromBaseOrDefault(this HtmlNode htmlNode, string classKey)
    {
        try
        {
            return htmlNode
                .SelectSingleNode($"//section[@class='{classKey}']");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return null;
        }
    }
    
    public static HtmlNode? TrySelectSingleNodeOrDefault(this HtmlNode htmlNode, string xpath, string classKey, string innerText)
    {
        try
        {
            return htmlNode
                .SelectSingleNode($"{xpath}//*[contains(@class, '{classKey}') and text()='{innerText}']");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return null;
        }
    }
    
    public static HtmlNode? TrySelectSingleNodeFromThisOrDefault(this HtmlNode htmlNode, string classKey)
    {
        try
        {
            return htmlNode
                .SelectSingleNode($"{htmlNode.XPath}//*[contains(@class, '{classKey}')]");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return null;
        }
    }
}