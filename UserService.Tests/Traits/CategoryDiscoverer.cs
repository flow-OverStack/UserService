using Xunit.Abstractions;
using Xunit.Sdk;

namespace UserService.Tests.Traits;

/// <summary>
/// Maps [UnitTest] on a test class to the "Category"="Unit" trait, so
/// `dotnet test --filter Category=Unit` keeps working without repeating
/// [Trait("Category", "Unit")] on every test method.
/// </summary>
public sealed class UnitTestDiscoverer : ITraitDiscoverer
{
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        yield return new KeyValuePair<string, string>("Category", "Unit");
    }
}

/// <summary>
/// Maps [FunctionalTest] on a test class to the "Category"="Functional" trait, so
/// `dotnet test --filter Category=Functional` keeps working without repeating
/// [Trait("Category", "Functional")] on every test method.
/// </summary>
public sealed class FunctionalTestDiscoverer : ITraitDiscoverer
{
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        yield return new KeyValuePair<string, string>("Category", "Functional");
    }
}
