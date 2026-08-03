using UserService.Domain.Entities;
using UserService.Domain.Helpers;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class UsernameSanitizerTests
{
    [Trait("Category", "Unit")]
    [Theory]
    [InlineData("TestUser", "testuser")]
    [InlineData("test@user", "test_user")]
    [InlineData("test__user", "test_user")]
    [InlineData("test---user", "test_user")]
    [InlineData("_test_", "test")]
    [InlineData("-test-", "test")]
    [InlineData("...test...", "test")]
    [InlineData("Test.User-1_2", "test.user-1_2")]
    public void Sanitize_ShouldBe_ExpectedResult(string raw, string expected)
    {
        //Act
        var result = UsernameSanitizer.Sanitize(raw);

        //Assert
        Assert.Equal(expected, result);
    }

    [Trait("Category", "Unit")]
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("!@#$%^")]
    public void Sanitize_AllDisallowedOrBlank_ShouldBe_Empty(string raw)
    {
        //Act
        var result = UsernameSanitizer.Sanitize(raw);

        //Assert
        Assert.Equal(string.Empty, result);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void Sanitize_OverLength_ShouldBe_TruncatedToMaxLength()
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
