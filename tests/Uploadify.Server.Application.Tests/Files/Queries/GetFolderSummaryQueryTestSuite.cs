using FluentAssertions;
using MediatR;
using Moq;
using Uploadify.Server.Application.Files.Models;
using Uploadify.Server.Application.Files.Queries;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Files.Models;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Tests.Common.Moq.Helpers;
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Application.Tests.Files.Queries;

public class GetFolderSummaryQueryTestSuite
{
    private readonly List<File> _files =
    [
        new() { Id = 1, FolderId = 1, UnsafeName = "FirstFile", Size = 128 },
        new() { Id = 2, FolderId = 2, UnsafeName = "SecondFile", Size = 256 }
    ];

    private readonly List<FolderLink> _folderLinks =
    [
        new() { FolderId = 1, ParentId = null, Name = "RootFolder" },
        new() { FolderId = 2, ParentId = 1, Name = "FirstSubfolder" },
        new() { FolderId = 3, ParentId = 1, Name = "SecondSubfolder" }
    ];

    private readonly List<Folder> _folders =
    [
        new()
        {
            Id = 1,
            ParentId = null,
            Name = "RootFolder",
            UserId = "ce563d48-0038-41f9-8eac-010820d9cd28",
            TotalCount = 3,
            TotalSize = 1024,
            Files =
            [
                new() { Id = 1, FolderId = 1, UnsafeName = "FirstFile", Size = 128 }
            ],
            Children =
            [
                new()
                {
                    Id = 2,
                    ParentId = 1,
                    Name = "FirstSubfolder",
                    UserId = "ce563d48-0038-41f9-8eac-010820d9cd28",
                    TotalCount = 1,
                    TotalSize = 256,
                    Files =
                    [
                        new() { Id = 2, FolderId = 2, UnsafeName = "SecondFile", Size = 256 }
                    ]
                },
                new() { Id = 3, ParentId = 1, Name = "SecondSubfolder", UserId = "ce563d48-0038-41f9-8eac-010820d9cd28", TotalCount = 2, TotalSize = 512 }
            ]
        },
        new()
        {
            Id = 2,
            ParentId = 1,
            Name = "FirstSubfolder",
            UserId = "ce563d48-0038-41f9-8eac-010820d9cd28",
            TotalCount = 1,
            TotalSize = 256,
            Files =
            [
                new() { Id = 2, FolderId = 2, UnsafeName = "SecondFile", Size = 256 }
            ]
        },
        new() { Id = 3, ParentId = 1, Name = "SecondSubfolder", UserId = "ce563d48-0038-41f9-8eac-010820d9cd28", TotalCount = 2, TotalSize = 512 }
    ];

    [Fact]
    public async Task Handle_WhenFolderExists_ThenReturnFolderSummary()
    {
        // Arrange
        const int folderId = 1;

        var user = new User { Id = "ce563d48-0038-41f9-8eac-010820d9cd28", UserName = "TestUser" };
        var mockDataContext = MockDataContextFactory
            .SetupDataContext(context => context.Folders, MockDataContextFactory.CreateMockDbSet(_folders))
            .SetupDbSet(context => context.Files, MockDataContextFactory.CreateMockDbSet(_files))
            .SetupDbSet(context => context.Users, MockDataContextFactory.CreateMockDbSet([user]));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFolderLinksQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFolderLinksQueryResponse(_folderLinks));

        var query = new GetFolderSummaryQuery { FolderId = folderId, UserName = user.UserName };
        var handler = new GetFolderSummaryQueryHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.Summary.Should().NotBeNull();
        response.Summary.FolderId.Should().Be(folderId);
        response.Summary.HasFiles.Should().BeTrue();
        response.Summary.HasFolders.Should().BeTrue();
        response.Summary.Links.Should().HaveCount(_folderLinks.Count);
        response.Failure.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenFoldeNotFound_ThenReturnNotFoundResponse()
    {
        // Arrange
        const int folderId = 1;

        var user = new User { Id = "ce563d48-0038-41f9-8eac-010820d9cd28", UserName = "TestUser" };
        var mockDataContext = MockDataContextFactory
            .SetupDataContext(context => context.Folders, MockDataContextFactory.CreateMockDbSet(_folders))
            .SetupDbSet(context => context.Files, MockDataContextFactory.CreateMockDbSet(_files))
            .SetupDbSet(context => context.Users, MockDataContextFactory.CreateMockDbSet([user]));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFolderLinksQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFolderLinksQueryResponse(_folderLinks));

        var query = new GetFolderSummaryQuery { FolderId = 1_000, UserName = user.UserName };
        var handler = new GetFolderSummaryQueryHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.NotFound);
        response.Failure.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThenReturnsNotFoundResponse()
    {
        // Arrange
        const int folderId = 1;

        var user = new User { Id = "ce563d48-0038-41f9-8eac-010820d9cd28", UserName = "TestUser" };
        var mockDataContext = MockDataContextFactory
            .SetupDataContext(context => context.Folders, MockDataContextFactory.CreateMockDbSet(_folders))
            .SetupDbSet(context => context.Files, MockDataContextFactory.CreateMockDbSet(_files))
            .SetupDbSet(context => context.Users, MockDataContextFactory.CreateMockDbSet([user]));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(Status.NotFound, new RequestFailure { Exception = new EntityNotFoundException(user.Id, nameof(User)), UserFriendlyMessage = Translations.RequestStatuses.NotFound }));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFolderLinksQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFolderLinksQueryResponse(_folderLinks));

        var query = new GetFolderSummaryQuery { FolderId = folderId, UserName = $"{user.UserName }{user.UserName }"};
        var handler = new GetFolderSummaryQueryHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.NotFound);
        response.Failure.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }

}
