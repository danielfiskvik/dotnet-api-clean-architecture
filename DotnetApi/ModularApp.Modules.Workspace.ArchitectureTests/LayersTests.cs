using ModularApp.Modules.Workspace.ArchitectureTests.Common;
using NetArchTest.Rules;
using NUnit.Framework;

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
}