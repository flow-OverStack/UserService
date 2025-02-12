namespace UserService.Tests.Configurations;

internal class TestException() : Exception(ErrorMessage)
{
    public const string ErrorMessage = "A test exception was thrown.";
}