using HtmlAgilityPack;
using ModularApp.Modules.Workspace.ArchitectureTests.Common;
using ModularApp.Modules.Workspace.Domain.Entities;
using NetArchTest.Rules;

namespace ModularApp.Modules.Workspace.ArchitectureTests;

public class LayersTests : TestBase
{
    [Test]
    public void DomainLayer_DoesNotHaveDependency_ToApplicationLayer()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        AssertArchTestResult(result);
    }
    
    [Test]
    public void DomainLayer_DoesNotHaveDependency_ToInfrastructureLayer()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        AssertArchTestResult(result);
    }

    [Test]
    public void ApplicationLayer_DoesNotHaveDependency_ToInfrastructureLayer()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        AssertArchTestResult(result);
    }
    
    // TODO Test with Person and check if it has more data
    // TODO Need to store original InnerHtml from WaniKani
    // TODO InnerHtml needs to be parsed to Anki
    [Test]
    public void MyTest()
    {
        const string kanjii = "一";
        const int level = 1;
        
        #region Fetch, parse, and make object from "/vocabulary/{kanji}
        var vocabularyResponseFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "MockFiles", "VocabularyResponse.html");
        var vocabularyResponseFileContent = File.ReadAllText(vocabularyResponseFilePath);

        // Use the file content in your test assertions or other operations
        Assert.NotNull(vocabularyResponseFileContent);
            
        var vocabularyHtmlDocument = new HtmlDocument();
        vocabularyHtmlDocument.LoadHtml(vocabularyResponseFileContent);
            
        var vocabularyCharacter = new Character
        {
            Characters = kanjii,
            Type = "Vocabulary",
            Source = "WaniKani",
            ExternalUrl = $"https://www.wanikani.com/vocabulary/{kanjii}",
            Level = level,
            LessonPosition = 3
        };
        
        vocabularyCharacter.CopDataFromHtmlDocument(vocabularyHtmlDocument);
        #endregion
        
        #region Fetch, parse, and make object from "/kanji"
        // Assign
        // Step 1. Fetch always "/kanji" first since it is kanji we are searching for (guarantee match)
        var kanjiResponseFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "MockFiles", "KanjiResponse.html");
        var kanjiResponseFileContent = File.ReadAllText(kanjiResponseFilePath);

        // Use the file content in your test assertions or other operations
        Assert.NotNull(kanjiResponseFileContent);
        
        // Act
        var kanjiHtmlDocument = new HtmlDocument();
        kanjiHtmlDocument.LoadHtml(kanjiResponseFileContent);

        var kanjiCharacter = new Character
        {
            Characters = kanjii,
            Type = "Kanji",
            Source = "WaniKani",
            ExternalUrl = $"https://www.wanikani.com/kanji/{kanjii}",
            Level = level,
            LessonPosition = 2
        };

        kanjiCharacter.CopDataFromHtmlDocument(kanjiHtmlDocument);
        
        // Assert
        Assert.NotNull(kanjiCharacter);
        #endregion

        #region Fetch, parse, and make object from "/radicals/{radicalName}
        var nodes = kanjiHtmlDocument.DocumentNode
            .TrySelectNodesFromBaseOrDefault("component-character component-character--radical");

        var radicalHref = nodes
            .Select(x => x.GetAttributeValue("href", ""))
            .FirstOrDefault();

        if (!string.IsNullOrEmpty(radicalHref))
        {
            var radicalResponseFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "MockFiles", "RadicalResponse.html");
            var radicalResponseFileContent = File.ReadAllText(radicalResponseFilePath);

            // Use the file content in your test assertions or other operations
            Assert.NotNull(radicalResponseFileContent);
            
            var radicalHtmlDocument = new HtmlDocument();
            radicalHtmlDocument.LoadHtml(radicalResponseFileContent);
            
            var radicalCharacter = new Character
            {
                Characters = kanjii,
                Type = "Radical",
                Source = "WaniKani",
                ExternalUrl = $"https://www.wanikani.com{radicalHref}",
                Level = level,
                LessonPosition = 1
            };
            
            radicalCharacter.CopDataFromHtmlDocument(radicalHtmlDocument);
        }
        #endregion
    }
}