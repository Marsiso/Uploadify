using FluentAssertions;
using MediatR;
using Moq;
using Uploadify.Server.Application.Files.Commands;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Core.Files.Queries;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Files.Models;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Tests.Common.Moq.Helpers;

namespace Uploadify.Server.Application.Tests.Files.Commands;

public class MoveFolderCommandTestSuite
{
    private readonly User _user;
    private readonly Folder _folder;

    public MoveFolderCommandTestSuite()
    {
        _user = new() { Id = Guid.NewGuid().ToString(), UserName = "TestUser" };
        _folder = new() { Id = 1, UserId = _user.Id, Name = "FolderName" };
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ThenReturnSuccessResponse()
    {
        // Arrange
        var destinationFolder = new Folder { Id = _folder.Id + 1, UserId = _user.Id };

        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Folders,
            MockDataContextFactory.CreateMockDbSet([_folder, destinationFolder]));

        mockDataContext.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(_user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFolderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFolderQueryResponse(_folder));

        var command = new MoveFolderCommand { UserName = _user.UserName, FolderId = _folder.Id, DestinationFolderId = destinationFolder.Id };
        var handler = new MoveFolderCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.Folder.Should().NotBeNull();
        response.Failure.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThenReturnNotFoundResponse()
    {
        // Arrange
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Folders,
            MockDataContextFactory.CreateMockDbSet(Enumerable.Empty<Folder>()));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(Status.NotFound, new RequestFailure { UserFriendlyMessage = Translations.RequestStatuses.NotFound, Exception = new EntityNotFoundException(_user.Id, nameof(User)) }));

        var command = new MoveFolderCommand { UserName = _user.UserName, FolderId = _folder.Id, DestinationFolderId = _folder.Id + 1 };
        var handler = new MoveFolderCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.NotFound);
        response.Folder.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_WhenFolderNotFound_ThenReturnNotFoundResponse()
    {
        // Arrange
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Folders,
            MockDataContextFactory.CreateMockDbSet(Enumerable.Empty<Folder>()));

        mockDataContext.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(_user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFolderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFolderQueryResponse(Status.NotFound, new RequestFailure { UserFriendlyMessage = Translations.RequestStatuses.NotFound, Exception = new EntityNotFoundException(_folder.Id.ToString(), nameof(Folder)) }));

        var command = new MoveFolderCommand { UserName = _user.UserName, FolderId = _folder.Id, DestinationFolderId = _folder.Id + 1 };
        var handler = new MoveFolderCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.NotFound);
        response.Folder.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }


    [Fact]
    public async Task Handle_WhenUnauthorizedAccess_ThenReturnUnauthorizedResponse()
    {
        // Arrange
        var folder = new Folder { Id = _folder.Id, UserId = Guid.NewGuid().ToString() };
        var destinationFolder = new Folder { Id = 2, UserId = _user.Id };
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Folders,
            MockDataContextFactory.CreateMockDbSet([folder, destinationFolder]));

        mockDataContext.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(_user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFolderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFolderQueryResponse(folder));

        var command = new MoveFolderCommand { UserName = _user.UserName, FolderId = folder.Id, DestinationFolderId = destinationFolder.Id };
        var handler = new MoveFolderCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Forbidden);
        response.Folder.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }
}
