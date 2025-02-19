using UserService.Tests.UnitTests.ServiceFactories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class ReputationResetServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task IncreaseReputation_ShouldBe_Success()
    {
        //Arrange
        var reputationService = new ReputationResetServiceFactory().GetService();

        //Act
        var result = await reputationService.ResetEarnedTodayReputation();

        //Assert
        Assert.True(result.IsSuccess);
    }
}