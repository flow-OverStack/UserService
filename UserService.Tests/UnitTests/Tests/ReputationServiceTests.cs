using UserService.Domain.Dtos.User;
using UserService.Domain.Resources;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class ReputationServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task IncreaseReputation_ShouldBe_Success()
    {
        //Arrange
        var reputationService = new ReputationServiceFactory().GetService();
        var dto = new ReputationIncreaseDto(1, 1);

        //Act
        var result = await reputationService.IncreaseReputationAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task IncreaseReputation_ShouldBe_NegativeReputation()
    {
        //Arrange
        var reputationService = new ReputationServiceFactory().GetService();
        var dto = new ReputationIncreaseDto(1, -1);

        //Act
        var result = await reputationService.IncreaseReputationAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.CannotIncreaseOrDecreaseNegativeReputation, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task IncreaseReputation_ShouldBe_UserNotFound()
    {
        //Arrange
        var reputationService = new ReputationServiceFactory().GetService();
        var dto = new ReputationIncreaseDto(0, 1);

        //Act
        var result = await reputationService.IncreaseReputationAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task IncreaseReputation_ShouldBe_DailyReputationLimitExceeded()
    {
        //Arrange
        var reputationService = new ReputationServiceFactory().GetService();
        var dto = new ReputationIncreaseDto(3, MockRepositoriesGetters.MaxDailyReputation + 1);

        //Act
        var result = await reputationService.IncreaseReputationAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.DailyReputationLimitExceeded, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DecreaseReputation_ShouldBe_Success()
    {
        //Arrange
        var reputationService = new ReputationServiceFactory().GetService();
        var dto = new ReputationDecreaseDto(3, 1);

        //Act
        var result = await reputationService.DecreaseReputationAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DecreaseReputation_ShouldBe_NegativeReputation()
    {
        //Arrange
        var reputationService = new ReputationServiceFactory().GetService();
        var dto = new ReputationDecreaseDto(1, -1);

        //Act
        var result = await reputationService.DecreaseReputationAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.CannotIncreaseOrDecreaseNegativeReputation, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DecreaseReputation_ShouldBe_UserNotFound()
    {
        //Arrange
        var reputationService = new ReputationServiceFactory().GetService();
        var dto = new ReputationDecreaseDto(0, 1);

        //Act
        var result = await reputationService.DecreaseReputationAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DecreaseReputation_ShouldBe_ReputationMinimumReached()
    {
        //Arrange
        var reputationService = new ReputationServiceFactory().GetService();
        var dto = new ReputationDecreaseDto(1, 1);

        //Act
        var result = await reputationService.DecreaseReputationAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationMinimumReached, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    // ResetEarnedTodayReputationAsync is not tested because
    // ExecuteUpdateAsync requires a real database
}