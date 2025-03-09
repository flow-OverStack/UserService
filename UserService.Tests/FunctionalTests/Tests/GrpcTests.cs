using Grpc.Core;
using Grpc.Net.Client;
using UserService.Domain.Resources;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class GrpcTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetUserById_ShouldBe_Success()
    {
        //Arrange
        const long userId = 1;
        var channel =
            GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });
        var client = new UserService.UserServiceClient(channel);

        //Act
        var user = await client.GetUserByIdAsync(new GetUserByIdRequest { UserId = userId });

        //Assert
        Assert.NotNull(user);
        Assert.NotNull(user.Roles);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetUserById_ShouldBe_UserNotFound()
    {
        //Arrange
        const long userId = 0;
        var channel =
            GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });
        var client = new UserService.UserServiceClient(channel);

        //Act
        var userRequest = async () => await client.GetUserByIdAsync(new GetUserByIdRequest { UserId = userId });

        //Assert
        var exception = await Assert.ThrowsAsync<RpcException>(userRequest);
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Equal(ErrorMessage.UserNotFound, exception.Status.Detail);
    }
}