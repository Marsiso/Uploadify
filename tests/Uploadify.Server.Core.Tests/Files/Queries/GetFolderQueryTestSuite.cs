using FluentAssertions;
using Uploadify.Server.Core.Files.Queries;
using Uploadify.Server.Domain.Files.Models;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Tests.Common.Moq.Helpers;

namespace Uploadify.Server.Core.Tests.Files.Queries;

public class GetFolderQueryTestSuite
{
    [Fact]
    public async Task Handle_WhenFolderExists_ReturnFile()
    {
        // Arrange
        var folder = new Folder { Id = 1 };
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Folders,
            MockDataContextFactory.CreateMockDbSet(new List<Folder>
            {
                folder,
                new() { Id = 2 },
                new() { Id = 3 }
            }.AsQueryable()));

        var query = new GetFolderQuery { FolderId = folder.Id };
        var handler = new GetFolderQueryHandler(mockDataContext.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.Folder.Should().NotBeNull();
        response.Folder.Should().BeEquivalentTo(folder);
        response.Folder.Id.Should().Be(folder.Id);
        response.Failure.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenFolderDoesNotExist_ReturnFailure()
    {
        // Arrange
        var folder = new Folder { Id = 1 };
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Folders,
            MockDataContextFactory.CreateMockDbSet(new List<Folder>
            {
                folder,
                new() { Id = 2 },
                new() { Id = 3 }
            }.AsQueryable()));

        var query = new GetFolderQuery { FolderId = 4 };
        var handler = new GetFolderQueryHandler(mockDataContext.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.NotFound);
        response.Folder.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }
}
