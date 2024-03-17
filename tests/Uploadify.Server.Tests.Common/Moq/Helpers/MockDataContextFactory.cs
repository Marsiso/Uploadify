using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Tests.Common.Moq.Queries;

namespace Uploadify.Server.Tests.Common.Moq.Helpers;

public static class MockDataContextFactory
{
    public static Mock<DbSet<TEntity>> CreateMockDbSet<TEntity>(IEnumerable<TEntity> source) where TEntity : class
    {
        var queryable = source.AsQueryable();
        var entities = new Mock<DbSet<TEntity>>();

        entities.As<IAsyncEnumerable<TEntity>>()
            .Setup(m => m.GetAsyncEnumerator(new CancellationToken()))
            .Returns(new AsyncEnumerator<TEntity>(queryable.GetEnumerator()));

        entities.As<IQueryable<TEntity>>()
            .Setup(m => m.Provider)
            .Returns(new AsyncQueryProvider<TEntity>(queryable.Provider));

        entities.As<IQueryable<TEntity>>()
            .Setup(m => m.Expression)
            .Returns(queryable.Expression);

        entities.As<IQueryable<TEntity>>()
            .Setup(m => m.ElementType)
            .Returns(queryable.ElementType);

        entities.As<IQueryable<TEntity>>()
            .Setup(m => m.GetEnumerator())
            .Returns(queryable.GetEnumerator());

        return entities;
    }

    public static Mock<DataContext> SetupDataContext<TEntity>(Mock<DbSet<TEntity>> entities) where TEntity : class
    {
        var context = new Mock<DataContext>(new DbContextOptions<DataContext>());

        context.Setup(context => context.Set<TEntity>()).Returns(entities.Object);

        return context;
    }

    public static Mock<DataContext> SetupDataContext<TEntity>(
        Expression<Func<DataContext, DbSet<TEntity>>> selector,
        Mock<DbSet<TEntity>> entities) where TEntity : class
    {
        var context = new Mock<DataContext>(new DbContextOptions<DataContext>());

        context.Setup(selector).Returns(entities.Object);

        return context;
    }

    public static Mock<DataContext> SetupDbSet<TEntity>(
        this Mock<DataContext> context,
        Expression<Func<DataContext, DbSet<TEntity>>> selector,
        Mock<DbSet<TEntity>> entities) where TEntity : class
    {
        context.Setup(selector).Returns(entities.Object);
        return context;
    }
}
