using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Enums;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class ReputationServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task ApplyReputationEvent_ShouldBe_UserNotFound()
    {
        //Arrange
        var reputationService = new ReputationServiceFactory().GetService();
        var dto = new ReputationEventDto(0, 1, EntityType.Answer, BaseEventType.AnswerUpvote);

        //Act
        var result = await reputationService.ApplyReputationEventAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task ApplyReputationEvent_ShouldBe_ReputationRuleNotFound()
    {
        //Arrange
        var reputationService = new ReputationServiceFactory().GetService();
        var dto = new ReputationEventDto(1, 1, EntityType.Answer, (BaseEventType)int.MaxValue); // Unknown event type

        //Act
        var result = await reputationService.ApplyReputationEventAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationRuleNotFound, result.ErrorMessage);
    }

    // Most of the methods are not tested because
    // ExecuteUpdateAsync requires a real database
}