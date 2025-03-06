using UserService.Domain.Resources;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.ServiceFactories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class GetUserServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllUsers_ShouldBe_Success()
    {
        //Arrange
        var getUserService = new GetUserServiceFactory().GetService();

        //Act
        var result = await getUserService.GetAllUsersAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(result.Count, result.Data.Count());
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllUsers_ShouldBe_UsersNotFound()
    {
        //Arrange
        var getUserService = new GetUserServiceFactory(MockRepositoriesGetters.GetEmptyMockUserRepository().Object)
            .GetService();

        //Act
        var result = await getUserService.GetAllUsersAsync();

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UsersNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserWithRole_ShouldBe_Success()
    {
        //Arrange
        var getUserService =
            new GetUserServiceFactory(roleRepository: MockRepositoriesGetters.GetMockRoleWithUsersRepository().Object)
                .GetService();
        const long roleId = 1;

        //Act
        var result = await getUserService.GetUsersWithRole(roleId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(result.Count, result.Data.Count());
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserWithRoles_ShouldBe_RoleNotFound()
    {
        //Arrange
        var getUserService = new GetUserServiceFactory().GetService();
        const long roleId = 4;

        //Act
        var result = await getUserService.GetUsersWithRole(roleId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }
}