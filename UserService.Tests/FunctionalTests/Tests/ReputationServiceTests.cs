using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class ReputationServiceTests(FunctionalTestWebAppFactory factory) : SequentialFunctionalTest(factory)
{
    // Most of the methods are tested here because
    // ExecuteUpdateAsync requires a real database

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ApplyReputationEvent_ShouldBe_ReputationRuleNotFound()
    {
        //Arrange
        var dto = new ReputationEventDto(1, 1, 1, EntityType.Answer, BaseEventType.EntityDownvoted);
        await using var scope = ServiceProvider.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<IReputationService>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRule>>();

        var rule = repository.GetAll().First(x => x.Id == 3); // Answer downvote rule
        repository.Remove(rule);
        await repository.SaveChangesAsync();

        //Act
        var result = await service.ApplyReputationEventAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationRuleNotFound, result.ErrorMessage);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ApplyReputationEvent_ShouldBe_UserNotFound()
    {
        //Arrange
        var dto = new ReputationEventDto(1, 0, 1, EntityType.Answer, BaseEventType.EntityDownvoted);
        await using var scope = ServiceProvider.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<IReputationService>();

        //Act
        var result = await service.ApplyReputationEventAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
    }
}