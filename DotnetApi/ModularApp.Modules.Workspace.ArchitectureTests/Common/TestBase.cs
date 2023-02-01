using System.Reflection;
using NetArchTest.Rules;

namespace ModularApp.Modules.Workspace.ArchitectureTests.Common;

public abstract class TestBase
{
    protected static Assembly DomainAssembly => typeof(Domain.Entities.Workspace).Assembly;
    protected static Assembly ApplicationAssembly => typeof(Application.ConfigureModule).Assembly;
    protected static Assembly InfrastructureAssembly => typeof(Infrastructure.ConfigureModule).Assembly;
    
    protected static void AssertFailingTypes(IEnumerable<Type> types)
    {
        Assert.That(types, Is.Null.Or.Empty);
    }

    protected static void AssertArchTestResult(TestResult result)
    {
        AssertFailingTypes(result.FailingTypes);
    }
}