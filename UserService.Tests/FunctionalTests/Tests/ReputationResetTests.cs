using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.BackgroundTasks.Jobs;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

[Collection("ReputationResetSequentialTests")]
public class ReputationResetTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task RunResetJob_ShouldBe_Success()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var reputationResetService = scope.ServiceProvider.GetRequiredService<ReputationResetJob>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();

        //Act
        await reputationResetService.Run();

        //Assert
        var reputations = await userRepository.GetAll().Select(x => x.ReputationEarnedToday).ToListAsync();

        Assert.True(reputations.All(x => x == 0));
    }
}