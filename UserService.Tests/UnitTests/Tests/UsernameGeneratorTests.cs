using UserService.Tests.UnitTests.Sut;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.UnitTests.Tests;

[UnitTest]
public class UsernameGeneratorTests
{
    [Fact]
    public async Task ResolveUniqueUsernameAsync_SanitizedAndFree_ReturnsNotTemporary()
    {
        //Arrange
        var usernameGenerator = new UsernameGeneratorSut().GetService();

        //Act
        var (username, isTemporary) = await usernameGenerator.ResolveUniqueUsernameAsync("NewUniqueUser");

        //Assert
        Assert.Equal("newuniqueuser", username);
        Assert.False(isTemporary);
    }

    [Fact]
    public async Task ResolveUniqueUsernameAsync_SanitizedButTaken_ReturnsTemporary()
    {
        //Arrange
        var usernameGenerator = new UsernameGeneratorSut().GetService();

        //Act
        var (username, isTemporary) = await usernameGenerator.ResolveUniqueUsernameAsync("testuser1");

        //Assert
        Assert.Equal("testuser1", username);
        Assert.True(isTemporary);
    }

    [Fact]
    public async Task ResolveUniqueUsernameAsync_Blank_ReturnsDefaultTemporary()
    {
        //Arrange
        var usernameGenerator = new UsernameGeneratorSut().GetService();

        //Act
        var (username, isTemporary) = await usernameGenerator.ResolveUniqueUsernameAsync("!@#$%^");

        //Assert
        Assert.Equal("user", username);
        Assert.True(isTemporary);
    }
}
