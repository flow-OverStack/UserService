namespace UserService.Tests.Support;

internal class TestException() : Exception(ErrorMessage)
{
    public const string ErrorMessage = "A test exception was thrown.";
}