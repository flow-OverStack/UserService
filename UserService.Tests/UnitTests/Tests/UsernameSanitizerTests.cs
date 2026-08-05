using UserService.Domain.Helpers;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.UnitTests.Tests;

[UnitTest]
public class UsernameSanitizerTests
{
    [Theory]
    [InlineData("TestUser", "testuser")]
    [InlineData("test@user", "test_user")]
    [InlineData("test__user", "test_user")]
    [InlineData("test---user", "test_user")]
    [InlineData("_test_", "test")]
    [InlineData("-test-", "test")]
    [InlineData("...test...", "test")]
    [InlineData("Test.User-1_2", "test.user-1_2")]
    public void Sanitize_MixedInput_ReturnsNormalizedUsername(string raw, string expected)
    {
        //Act
        var result = UsernameSanitizer.Sanitize(raw);

        //Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("!@#$%^")]
    public void Sanitize_AllDisallowedOrBlank_ReturnsEmpty(string raw)
    {
        //Act
        var result = UsernameSanitizer.Sanitize(raw);

        //Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Sanitize_OverMaxLength_ReturnsTruncated()
    {
        //Arrange
        var raw = new string('a', 30);

        //Act
        var result = UsernameSanitizer.Sanitize(raw);

        //Assert
        Assert.Equal(20, result.Length);
        Assert.Equal(new string('a', 20), result);
    }
}
