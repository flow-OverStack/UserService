using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.BackgroundTasks.Jobs;
using UserService.DAL;
using UserService.Domain.Entity;
using UserService.Tests.FunctionalTests.Base.Exception.ReputationResetJob;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class ExceptionReputationResetJobTests(ReputationResetJobExceptionFunctionalTestWebAppFactory factory)
    : ReputationResetJobExceptionBaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task RunResetJob_ShouldBe_NoException()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var reputationResetService = scope.ServiceProvider.GetRequiredService<ReputationResetJob>();
        //ApplicationDbContext is instead of IBaseRepository<User> because IBaseRepository<User> is mocked 
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        //Act
        await reputationResetService.Run();

        //Assert
        var reputations = await dbContext.Set<User>().Select(x => x.ReputationEarnedToday).ToListAsync();

        Assert.True(reputations.Any(x => x != 0));
    }
}