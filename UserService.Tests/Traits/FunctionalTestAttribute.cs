using Xunit.Sdk;

namespace UserService.Tests.Traits;

[TraitDiscoverer("UserService.Tests.Traits.FunctionalTestDiscoverer", "UserService.Tests")]
[AttributeUsage(AttributeTargets.Class)]
public sealed class FunctionalTestAttribute : Attribute, ITraitAttribute;
