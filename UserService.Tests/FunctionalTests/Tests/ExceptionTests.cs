using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using UserService.Api.Dtos.Role;
using UserService.Api.Dtos.UserRole;
using UserService.Application.Resources;
using UserService.Domain.Dtos.Role;
using UserService.Domain.Dtos.User;
using UserService.Domain.Dtos.UserRole;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.Messaging.Events;
using UserService.Messaging.Filters;
using UserService.Tests.Configurations;
using UserService.Tests.Constants;
using UserService.Tests.FunctionalTests.Base.Exception;
using UserService.Tests.FunctionalTests.Configurations.GraphQl;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class ExceptionTests : ExceptionBaseFunctionalTest
{
    public ExceptionTests(ExceptionFunctionalTestWebAppFactory factory) : base(factory)
    {
        var accessToken = TokenHelper.GetRsaTokenWithRoleClaims("testuser1", [
            new Role { Name = "User" },
            new Role { Name = "Admin" }
        ]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RegisterUser_ShouldBe_InternalServerError()
    {
        //Arrange
        var dto = new RegisterUserDto("TestUser4", "TestsUser4@test.com",
            TestConstants.TestPassword + "4");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Auth/register", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task InitUser_ShouldBe_InternalServerError()
    {
        //Arrange
        var accessToken =
            TokenHelper.GetRsaTokenWithIdentityData("testuser4", "TestUser4@test.com", "test-identity-id-4");
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //Act
        var response = await HttpClient.PostAsync("/api/v1.0/Auth/init", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DeleteRole_ShouldBe_InternalServerError()
    {
        //Arrange
        const long roleId = 3;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/role/{roleId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<RoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateRole_ShouldBe_InternalServerError()
    {
        //Arrange
        const long roleId = 3;
        var dto = new RequestRoleDto("UpdatedTestRole");

        //Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1.0/Role/{roleId}", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<RoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_InternalServerError()
    {
        //Arrange
        const string username = "TestUser1";
        const long roleId = 3;

        //Act
        var response = await HttpClient.PostAsync($"/api/v1.0/role/{username}/{roleId}", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserRoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_InternalServerError()
    {
        //Arrange
        const string username = "TestUser2";
        const long roleId = 3;

        //Act
        var response =
            await HttpClient.DeleteAsync($"/api/v1.0/role/{username}/{roleId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserRoleDto>>(body);


        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_InternalServerError()
    {
        //Arrange
        const string username = "TestUser2";
        var dto = new RequestUpdateUserRoleDto
        {
            FromRoleId = 3,
            ToRoleId = 2
        };

        //Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1.0/role/" + username, dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserRoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetUserById_ShouldBe_Ok()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestUserByIdQuery(1) };

        //Act
        // 1st request fetches data from DB
        await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        // 2nd request fetches data from cache
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetUserByIdResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.User);
        Assert.NotNull(result.Data.User.Roles);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeBaseEvent_ShouldBe_Exception()
    {
        //Arrange
        var dto = new ReputationEventDto(1, 1, EntityType.Answer, BaseEventType.AnswerAccepted);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var reputationService = scope.ServiceProvider.GetRequiredService<IReputationService>();

        //Act
        var action = () => reputationService.ApplyReputationEventAsync(dto);

        //Assert
        await Assert.ThrowsAsync<TestException>(action);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Send_ShouldBe_Exception()
    {
        //Arrange
        const long userId = 1;
        const long entityId = 2;

        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.AnswerAccepted),
            EntityType = nameof(EntityType.Answer),
            UserId = userId,
            EntityId = entityId,
            EventId = Guid.NewGuid()
        };
        await using var scope = ServiceProvider.CreateAsyncScope();
        var filter = ActivatorUtilities.CreateInstance<ProcessedEventFilter<BaseEvent>>(scope.ServiceProvider);

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        var pipeMock = new Mock<IPipe<ConsumeContext<BaseEvent>>>();

        //Act
        var action = () => filter.Send(contextMock.Object, pipeMock.Object);

        //Assert
        await Assert.ThrowsAsync<TestException>(action);
    }
}