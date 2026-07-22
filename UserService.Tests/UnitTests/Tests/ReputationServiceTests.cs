using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Enums;
using UserService.Tests.UnitTests.Sut;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.UnitTests.Tests;

[UnitTest]
public class ReputationServiceTests
{
    // Most of the methods are not tested because
    // ExecuteUpdateAsync requires a real database

    [Fact]
    public async Task ApplyReputationEventAsync_UnknownEventType_ReturnsReputationRulesNotFound()
    {
        //Arrange
        var reputationService = new ReputationServiceSut().GetService();
        var dto = new ReputationEventDto(1, 2, 1, EntityType.Answer, (BaseEventType)int.MaxValue); // Unknown event type

        //Act
        var result = await reputationService.ApplyReputationEventAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationRulesNotFound, result.ErrorMessage);
    }
}