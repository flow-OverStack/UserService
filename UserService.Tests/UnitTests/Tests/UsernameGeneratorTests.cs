using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class UsernameGeneratorTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task ResolveUniqueUsernameAsync_SanitizedAndFree_ShouldBe_NotTemporary()
    {
        //Arrange
        var usernameGenerator = new UsernameGeneratorFactory().GetService();

        //Act
        var (username, isTemporary) = await usernameGenerator.ResolveUniqueUsernameAsync("NewUniqueUser");

        //Assert
        Assert.Equal("newuniqueuser", username);
        Assert.False(isTemporary);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task ResolveUniqueUsernameAsync_SanitizedButTaken_ShouldBe_Temporary()
    {
        //Arrange
        var usernameGenerator = new UsernameGeneratorFactory().GetService();

        //Act
        var (username, isTemporary) = await usernameGenerator.ResolveUniqueUsernameAsync("testuser1");

        //Assert
        Assert.Equal("testuser1", username);
        Assert.True(isTemporary);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task ResolveUniqueUsernameAsync_Blank_ShouldBe_DefaultTemporary()
    {
        //Arrange
        var usernameGenerator = new UsernameGeneratorFactory().GetService();

        //Act
        var (username, isTemporary) = await usernameGenerator.ResolveUniqueUsernameAsync("!@#$%^");

        //Assert
        Assert.Equal("user", username);
        Assert.True(isTemporary);
    }
}
