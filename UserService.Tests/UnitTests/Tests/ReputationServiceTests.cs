using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Enums;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class ReputationServiceTests
{
    // Most of the methods are not tested because
    // ExecuteUpdateAsync requires a real database

    [Trait("Category", "Unit")]
    [Fact]
    public async Task ApplyReputationEvent_ShouldBe_ReputationRulesNotFound()
    {
        //Arrange
        var reputationService = new ReputationServiceFactory().GetService();
        var dto = new ReputationEventDto(1, 2, 1, EntityType.Answer, (BaseEventType)int.MaxValue); // Unknown event type

        //Act
        var result = await reputationService.ApplyReputationEventAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationRulesNotFound, result.ErrorMessage);
    }
}