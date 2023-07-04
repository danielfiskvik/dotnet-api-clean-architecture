using ModularApp.Modules.Workspace.Application.Services;
using ModularApp.Modules.Workspace.ArchitectureTests.Common;
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
    
    // TODO Test with an url you know does not work..
    // TODO Test with Person and check if it has more data
    [Test]
    public void MyTest()
    {
        var service = new CharacterMetadataService(default, default);
        
        service.FetchDataAsync(default, default, default, default);
    }
}