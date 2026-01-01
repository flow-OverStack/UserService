using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class ReputationRuleDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Success()
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

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Null()
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