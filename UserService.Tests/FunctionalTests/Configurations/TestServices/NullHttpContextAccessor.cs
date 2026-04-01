using Microsoft.AspNetCore.Http;

namespace UserService.Tests.FunctionalTests.Configurations.TestServices;

internal class NullHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext? HttpContext
    {
        get => null;
        set { }
    }
}