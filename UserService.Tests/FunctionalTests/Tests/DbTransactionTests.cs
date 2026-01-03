using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

[Collection(nameof(DbTransactionTests))]
public class DbTransactionTests(FunctionalTestWebAppFactory factory) : SequentialFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task CommitTransaction_ShouldBe_Ok()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        //Act
        await using var transaction = await unitOfWork.BeginTransactionAsync();
        var user = new User
        {
            Username = "testuser",
            Email = "transactionuser@test.com",
            IdentityId = Guid.NewGuid().ToString()
        };
        await unitOfWork.Users.CreateAsync(user);
        await unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();

        //Assert
        var count = await unitOfWork.Users.GetAll().AsNoTracking().CountAsync();
        Assert.Equal(4, count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RollbackTransaction_ShouldBe_Ok()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        //Act
        await using var transaction = await unitOfWork.BeginTransactionAsync();
        var user = new User
        {
            Username = "transactionuser",
            Email = "transactionuser@test.com",
            IdentityId = Guid.NewGuid().ToString()
        };
        await unitOfWork.Users.CreateAsync(user);
        await unitOfWork.SaveChangesAsync();
        await transaction.RollbackAsync();

        //Assert
        var count = await unitOfWork.Users.GetAll().AsNoTracking().CountAsync();
        Assert.Equal(3, count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DisposeTransaction_ShouldBe_Ok()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        //Act
        var transaction = await unitOfWork.BeginTransactionAsync();
        var user = new User
        {
            Username = "transactionuser",
            Email = "transactionuser@test.com",
            IdentityId = Guid.NewGuid().ToString()
        };
        await unitOfWork.Users.CreateAsync(user);
        await unitOfWork.SaveChangesAsync();
        await transaction.DisposeAsync();

        //Assert
        var count = await unitOfWork.Users.GetAll().AsNoTracking().CountAsync();
        Assert.Equal(3, count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task CommitNestedTransaction_ShouldBe_Ok()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        //Act
        await using var transaction1 = await unitOfWork.BeginTransactionAsync();

        await using var transaction2 = await unitOfWork.BeginTransactionAsync();
        var user2 = new User
        {
            Username = "transactionuser2",
            Email = "transactionuser2@test.com",
            IdentityId = Guid.NewGuid().ToString()
        };
        await unitOfWork.Users.CreateAsync(user2);
        await unitOfWork.SaveChangesAsync();
        await transaction2.CommitAsync();

        var user1 = new User
        {
            Username = "transactionuser1",
            Email = "transactionuser1@test.com",
            IdentityId = Guid.NewGuid().ToString()
        };
        await unitOfWork.Users.CreateAsync(user1);
        await unitOfWork.SaveChangesAsync();
        await transaction1.CommitAsync();

        //Assert
        var count = await unitOfWork.Users.GetAll().AsNoTracking().CountAsync();
        Assert.Equal(5, count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RollbackNestedTransaction_ShouldBe_Ok()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        //Act
        await using var transaction1 = await unitOfWork.BeginTransactionAsync();

        await using var transaction2 = await unitOfWork.BeginTransactionAsync();
        var user2 = new User
        {
            Username = "transactionuser2",
            Email = "transactionuser2@test.com",
            IdentityId = Guid.NewGuid().ToString()
        };
        await unitOfWork.Users.CreateAsync(user2);
        await unitOfWork.SaveChangesAsync();
        await transaction2.RollbackAsync();

        var user1 = new User
        {
            Username = "transactionuser1",
            Email = "transactionuser1@test.com",
            IdentityId = Guid.NewGuid().ToString()
        };
        await unitOfWork.Users.CreateAsync(user1);
        await unitOfWork.SaveChangesAsync();
        await transaction1.CommitAsync();

        //Assert
        var count = await unitOfWork.Users.GetAll().AsNoTracking().CountAsync();
        Assert.Equal(4, count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DisposeNestedTransaction_ShouldBe_Ok()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        //Act
        await using var transaction1 = await unitOfWork.BeginTransactionAsync();

        var transaction2 = await unitOfWork.BeginTransactionAsync();
        var user2 = new User
        {
            Username = "transactionuser2",
            Email = "transactionuser2@test.com",
            IdentityId = Guid.NewGuid().ToString()
        };
        await unitOfWork.Users.CreateAsync(user2);
        await unitOfWork.SaveChangesAsync();
        await transaction2.DisposeAsync();

        var user1 = new User
        {
            Username = "transactionuser1",
            Email = "transactionuser1@test.com",
            IdentityId = Guid.NewGuid().ToString()
        };
        await unitOfWork.Users.CreateAsync(user1);
        await unitOfWork.SaveChangesAsync();
        await transaction1.CommitAsync();

        //Assert
        var count = await unitOfWork.Users.GetAll().AsNoTracking().CountAsync();
        Assert.Equal(4, count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RollbackTransaction_ShouldBe_Ok_With_NestedTransaction()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        //Act
        await using var transaction1 = await unitOfWork.BeginTransactionAsync();

        await using var transaction2 = await unitOfWork.BeginTransactionAsync();
        var user2 = new User
        {
            Username = "transactionuser2",
            Email = "transactionuser2@test.com",
            IdentityId = Guid.NewGuid().ToString()
        };
        await unitOfWork.Users.CreateAsync(user2);
        await unitOfWork.SaveChangesAsync();
        await transaction2.CommitAsync();

        var user1 = new User
        {
            Username = "transactionuser1",
            Email = "transactionuser1@test.com",
            IdentityId = Guid.NewGuid().ToString()
        };
        await unitOfWork.Users.CreateAsync(user1);
        await unitOfWork.SaveChangesAsync();
        await transaction1.RollbackAsync();

        //Assert
        var count = await unitOfWork.Users.GetAll().AsNoTracking().CountAsync();
        Assert.Equal(3, count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DisposeTransaction_ShouldBe_Ok_With_NestedTransaction()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        //Act
        var transaction1 = await unitOfWork.BeginTransactionAsync();

        await using var transaction2 = await unitOfWork.BeginTransactionAsync();
        var user2 = new User
        {
            Username = "transactionuser2",
            Email = "transactionuser2@test.com",
            IdentityId = Guid.NewGuid().ToString()
        };
        await unitOfWork.Users.CreateAsync(user2);
        await unitOfWork.SaveChangesAsync();
        await transaction2.CommitAsync();

        var user1 = new User
        {
            Username = "transactionuser1",
            Email = "transactionuser1@test.com",
            IdentityId = Guid.NewGuid().ToString()
        };
        await unitOfWork.Users.CreateAsync(user1);
        await unitOfWork.SaveChangesAsync();
        await transaction1.DisposeAsync();

        //Assert
        var count = await unitOfWork.Users.GetAll().AsNoTracking().CountAsync();
        Assert.Equal(3, count);
    }
}