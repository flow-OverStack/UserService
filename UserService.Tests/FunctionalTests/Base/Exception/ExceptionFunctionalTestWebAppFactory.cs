using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using UserService.DAL.Repositories;
using UserService.Domain.Interfaces.Repositories;
using UserService.Tests.Configurations;

namespace UserService.Tests.FunctionalTests.Base.Exception;

public class ExceptionFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private static Mock<IDbContextTransaction> GetExceptionMockTransaction(IDbContextTransaction originalTransaction)
    {
        var mockTransaction = new Mock<IDbContextTransaction>();

        mockTransaction.Setup(x => x.RollbackAsync(default))
            .Returns(originalTransaction.RollbackAsync);
        mockTransaction.Setup(x => x.CommitAsync(default)).ThrowsAsync(new TestException());

        return mockTransaction;
    }

    private static async Task<Mock<IUnitOfWork>> GetExceptionMockUnitOfWork(IUnitOfWork originalUnitOfWork)
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        var originalTransaction = await originalUnitOfWork.BeginTransactionAsync();
        mockUnitOfWork.Setup(x => x.SaveChangesAsync()).Returns(originalUnitOfWork.SaveChangesAsync);
        mockUnitOfWork.Setup(x => x.BeginTransactionAsync())
            .ReturnsAsync(GetExceptionMockTransaction(originalTransaction).Object);
        mockUnitOfWork.Setup(x => x.Users).Returns(originalUnitOfWork.Users);
        mockUnitOfWork.Setup(x => x.UserRoles).Returns(originalUnitOfWork.UserRoles);

        return mockUnitOfWork;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IUnitOfWork>();
            services.AddScoped<IUnitOfWork>(provider =>
            {
                //the dependencies from service provider only apply for this current scope
                //that is why we have to use ActivatorUtilities to transfer dependencies from this scope to callers' scope
                var unitOfWork = ActivatorUtilities.CreateInstance<UnitOfWork>(provider);
                var exceptionUnitOfWork = GetExceptionMockUnitOfWork(unitOfWork).GetAwaiter().GetResult().Object;

                return exceptionUnitOfWork;
            });
        });
    }
}