namespace UserService.Tests.Configurations;

public class TestException() : Exception(ErrorMessage)
{
    private const string ErrorMessage = "A test exception was thrown.";
}