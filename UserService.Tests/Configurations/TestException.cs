namespace UserService.Tests.Configurations;

internal class TestException() : Exception(ErrorMessage)
{
    private const string ErrorMessage = "A test exception was thrown.";
}