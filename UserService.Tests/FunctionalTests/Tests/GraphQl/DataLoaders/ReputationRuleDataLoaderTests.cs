using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

[FunctionalTest]
public class ReputationRuleDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task LoadAsync_ExistingRuleId_ReturnsSuccess()
    {
        //Arrange        
        const long ruleId = 1;
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<ReputationRuleDataLoader>();

        //Act
        var result = await dataLoader.LoadAsync(ruleId);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(ruleId, result.Id);
    }

    [Fact]
    public async Task LoadAsync_NonExistentRuleId_ReturnsNull()
    {
        //Arrange        
        const long ruleId = 0;
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<ReputationRuleDataLoader>();

        //Act
        var result = await dataLoader.LoadAsync(ruleId);

        //Assert
        Assert.Null(result);
    }
}