using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain.Entities;
using Infrastructure.Persistence;
using Domain.Enums;
using Domain.ValueObjects;
using System.Collections.Generic;

namespace Infrastructure.Tests;

public class WFCaseRepositoryTests
{
    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<WFCaseLink> WFCaseLinks { get; set; }
    }

    private WFCaseRepositoryBase<TestDbContext> CreateRepository(TestDbContext context) =>
        new WFCaseRepositoryBase<TestDbContext>(context);

    private TestDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new TestDbContext(options);
    }

    [Fact]
    public async Task AddAndGetById_ShouldWork()
    {
        using var context = CreateInMemoryContext("AddAndGetByIdDb");
        var repo = CreateRepository(context);

        var entity = new WFCaseLink
        {
            Id = Guid.NewGuid(),
            TargetCaseId = 1,
            currentTaskId = 100
        };

        await repo.AddAsync(entity);

        var fetched = await repo.GetByIdAsync(entity.Id);
        Assert.NotNull(fetched);
        Assert.Equal(entity.TargetCaseId, fetched.TargetCaseId);
        Assert.Equal(entity.currentTaskId, fetched.currentTaskId);
    }

    [Fact]
    public async Task GetByTaskId_ShouldReturnCorrectEntity()
    {
        using var context = CreateInMemoryContext("GetByTaskIdDb");
        var repo = CreateRepository(context);

        var entity = new WFCaseLink { Id = Guid.NewGuid(), currentTaskId = 200 };
        await repo.AddAsync(entity);

        var fetched = await repo.GetByTaskIdAsync(200);
        Assert.NotNull(fetched);
        Assert.Equal(entity.Id, fetched.Id);

        var nonExisting = await repo.GetByTaskIdAsync(999);
        Assert.Null(nonExisting);
    }

    [Fact]
    public async Task GetByCaseId_ShouldReturnCorrectEntity()
    {
        using var context = CreateInMemoryContext("GetByCaseIdDb");
        var repo = CreateRepository(context);

        var entity = new WFCaseLink { Id = Guid.NewGuid(), TargetCaseId = 300 };
        await repo.AddAsync(entity);

        var fetched = await repo.GetByCaseIdAsync(300);
        Assert.NotNull(fetched);
        Assert.Equal(entity.Id, fetched.Id);

        var nonExisting = await repo.GetByCaseIdAsync(999);
        Assert.Null(nonExisting);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        using var context = CreateInMemoryContext("UpdateDb");
        var repo = CreateRepository(context);

        var entity = new WFCaseLink { Id = Guid.NewGuid(), TargetCaseId = 1, currentTaskId = 10 };
        await repo.AddAsync(entity);

        entity.TargetCaseId = 2;
        entity.currentTaskId = 20;
        await repo.UpdateAsync(entity);

        var fetched = await repo.GetByIdAsync(entity.Id);
        Assert.Equal(2, fetched.TargetCaseId);
        Assert.Equal(20, fetched.currentTaskId);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        using var context = CreateInMemoryContext("DeleteDb");
        var repo = CreateRepository(context);

        var entity = new WFCaseLink { Id = Guid.NewGuid(), TargetCaseId = 1 };
        await repo.AddAsync(entity);

        await repo.DeleteAsync(entity.Id);
        var fetched = await repo.GetByIdAsync(entity.Id);
        Assert.Null(fetched);
    }

    [Fact]
    public async Task UpdateWFStateToFailed_ShouldSetFailedStatus()
    {
        using var context = CreateInMemoryContext("FailedDb");
        var repo = CreateRepository(context);

        var entity = new WFCaseLink { Id = Guid.NewGuid(), Status = WFCaseLinkStatus.Created };
        await repo.AddAsync(entity);

        await repo.UpdateWFStateToFailed(entity);
        var fetched = await repo.GetByIdAsync(entity.Id);
        Assert.Equal(WFCaseLinkStatus.Failed, fetched.Status);
    }

    [Fact]
    public async Task GetBySourceCaseIdAsync_ShouldReturnAllMatching()
    {
        using var context = CreateInMemoryContext("SourceCaseDb");
        var repo = CreateRepository(context);

        var list = new List<WFCaseLink>
        {
            new WFCaseLink { Id = Guid.NewGuid(), SourceCaseId = 1 },
            new WFCaseLink { Id = Guid.NewGuid(), SourceCaseId = 1 },
            new WFCaseLink { Id = Guid.NewGuid(), SourceCaseId = 2 }
        };
        foreach (var item in list) await repo.AddAsync(item);

        var fetched = await repo.GetBySourceCaseIdAsync(1);
        Assert.Equal(2, fetched.Count);
    }

    //[Fact]
    //public async Task UpdateRetryStateAsync_ShouldHandleSuccessAndFailure()
    //{
    //    using var context = CreateInMemoryContext("RetryDb");
    //    var repo = CreateRepository(context);

    //    var entity = new WFCaseLink
    //    {
    //        Id = Guid.NewGuid(),
    //        Status = WFCaseLinkStatus.Created,
    //        ProcessMetaData = new ProcessMetaData
    //        {
    //            Retry = new RetryMeta { RetryCount = 2, RetryAfterMinutes = 1 },
    //            IncidentDetails = new List<IncidentDetails>()
    //        }
    //    };
    //    await repo.AddAsync(entity);

    //    // Success
    //    await repo.UpdateRetryStateAsync(entity, true, CancellationToken.None);
    //    var fetched = await repo.GetByIdAsync(entity.Id);
    //    Assert.Equal(WFCaseLinkStatus.Completed, fetched.Status);
    //    Assert.Equal(0, fetched.ProcessMetaData.Retry.RetryCount);

    //    // Failure
    //    entity.ProcessMetaData.Retry.RetryCount = 1;
    //    await repo.UpdateRetryStateAsync(entity, false, CancellationToken.None);
    //    fetched = await repo.GetByIdAsync(entity.Id);
    //    Assert.Equal(WFCaseLinkStatus.Failed, fetched.Status);
    //    Assert.Equal(0, fetched.ProcessMetaData.Retry.RetryCount);
    //    Assert.Single(fetched.ProcessMetaData.IncidentDetails);
    //}

    [Fact]
    public async Task GetFailedLinksAsync_ShouldReturnOnlyFailed()
    {
        using var context = CreateInMemoryContext("FailedLinksDb");
        var repo = CreateRepository(context);

        var list = new List<WFCaseLink>
        {
            new WFCaseLink { Id = Guid.NewGuid(), Status = WFCaseLinkStatus.Failed },
            new WFCaseLink { Id = Guid.NewGuid(), Status = WFCaseLinkStatus.Completed },
            new WFCaseLink { Id = Guid.NewGuid(), Status = WFCaseLinkStatus.Failed }
        };
        foreach (var item in list) await repo.AddAsync(item);

        var failed = await repo.GetFailedLinksAsync(default);
        Assert.Equal(2, failed.Count());
    }
}
