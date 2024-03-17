using FluentAssertions;
using Uploadify.Server.Application.Files.Models;
using Uploadify.Server.Application.Files.Queries;
using Uploadify.Server.Domain.Files.Models;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Tests.Common.Moq.Helpers;

namespace Uploadify.Server.Application.Tests.Files.Queries;

public class GetFolderLinksQueryTestSuite
{
    [Fact]
    public async Task Handle_WhenFolderWithLinksExists_ThenReturnSuccessResponse()
    {
        // Arrange
        const int folderId = 1;
        var folderLinks = new List<FolderLink>
        {
            new() { FolderId = folderId, ParentId = 2, Name = "SecondSubfolder" },
            new() { FolderId = 2, ParentId = 3, Name = "FirstSubfolder" },
            new() { FolderId = 3, ParentId = null, Name = "RootFolder" }
        };

        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Folders,
            MockDataContextFactory.CreateMockDbSet(new List<Folder>
            {
                new() { Id = 1, ParentId = 2, Name = "SecondSubfolder" },
                new() { Id = 2, ParentId = 3, Name = "FirstSubfolder" },
                new() { Id = 3, ParentId = null, Name = "RootFolder" }
            }.AsQueryable()));

        var query = new GetFolderLinksQuery(folderId);
        var handler = new GetFolderLinksQueryHandler(mockDataContext.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.Links.Should().BeEquivalentTo(folderLinks);
        response.Failure.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenFolderHasNoSubfolders_ThenReturnSingleFolderLink()
    {
        // Arrange
        const int folderId = 1;

        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Folders,
            MockDataContextFactory.CreateMockDbSet(new List<Folder>
            {
                new() { Id = folderId, ParentId = null, Name = "SingleFolder" }
            }.AsQueryable()));

        var query = new GetFolderLinksQuery(folderId);
        var handler = new GetFolderLinksQueryHandler(mockDataContext.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.Links.SingleOrDefault().Should().NotBeNull();
        response.Links.Single().FolderId.Should().Be(folderId);
        response.Failure.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenFolderNotFound_ThenThrowException()
    {
        // Arrange
        const int folderId = 1;

        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Folders,
            MockDataContextFactory.CreateMockDbSet(new List<Folder>().AsQueryable()));

        var query = new GetFolderLinksQuery(folderId);
        var handler = new GetFolderLinksQueryHandler(mockDataContext.Object);

        // Act
        var exception = Record.ExceptionAsync(() => handler.Handle(query, CancellationToken.None));

        // Assert
        exception.Should().NotBeNull();
    }
}
