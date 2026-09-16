using Xunit.Sdk;

namespace UserService.Tests.Traits;

[TraitDiscoverer("UserService.Tests.Traits.UnitTestDiscoverer", "UserService.Tests")]
[AttributeUsage(AttributeTargets.Class)]
public sealed class UnitTestAttribute : Attribute, ITraitAttribute;
