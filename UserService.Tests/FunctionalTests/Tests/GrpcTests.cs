using Grpc.Core;
using Grpc.Net.Client;
using UserService.Application.Resources;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class GrpcTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetUserWithRolesById_ShouldBe_Ok()
    {
        //Arrange
        const long userId = 1;
        var channel =
            GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });
        var client = new UserService.UserServiceClient(channel);

        //Act
        var user = await client.GetUserWithRolesByIdAsync(new GetUserByIdRequest { Id = userId });

        //Assert
        Assert.NotNull(user);
        Assert.NotNull(user.Roles);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetUserWithRolesById_ShouldBe_UserNotFound()
    {
        //Arrange
        const long userId = 0;
        var channel =
            GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });
        var client = new UserService.UserServiceClient(channel);

        //Act
        var userRequest = async () =>
            await client.GetUserWithRolesByIdAsync(new GetUserByIdRequest { Id = userId });

        //Assert
        var exception = await Assert.ThrowsAsync<RpcException>(userRequest);
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Equal(ErrorMessage.UserNotFound, exception.Status.Detail);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetUsersByIds_ShouldBe_Ok()
    {
        //Arrange
        var userIds = new List<long> { 1, 2, 0 };
        var channel =
            GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });
        var client = new UserService.UserServiceClient(channel);

        var request = new GetUsersByIdsRequest();
        request.Ids.AddRange(userIds);

        //Act
        var response = await client.GetUsersByIdsAsync(request);

        //Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Users.Count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetUsersByIds_ShouldBe_UserNotFound()
    {
        //Arrange
        var userIds = new List<long> { 0 };
        var channel =
            GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });
        var client = new UserService.UserServiceClient(channel);

        var request = new GetUsersByIdsRequest();
        request.Ids.AddRange(userIds);

        //Act
        var usersRequest = async () => await client.GetUsersByIdsAsync(request);

        //Assert
        var exception = await Assert.ThrowsAsync<RpcException>(usersRequest);
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Equal(ErrorMessage.UserNotFound, exception.Status.Detail);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetUsersByIds_ShouldBe_UsersNotFound()
    {
        //Arrange
        var userIds = new List<long> { 0, -1 };
        var channel =
            GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });
        var client = new UserService.UserServiceClient(channel);

        var request = new GetUsersByIdsRequest();
        request.Ids.AddRange(userIds);

        //Act
        var usersRequest = async () => await client.GetUsersByIdsAsync(request);

        //Assert
        var exception = await Assert.ThrowsAsync<RpcException>(usersRequest);
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Equal(ErrorMessage.UsersNotFound, exception.Status.Detail);
    }
}