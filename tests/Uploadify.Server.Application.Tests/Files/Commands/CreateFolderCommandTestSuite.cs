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

public class CreateFolderCommandTestSuite
{
    private readonly User _user;
    private readonly Folder _parentFolder;

    public CreateFolderCommandTestSuite()
    {
        _user = new() { Id = Guid.NewGuid().ToString(), UserName = "TestUser" };
        _parentFolder = new() { Id = 1, UserId = _user.Id };
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ThenReturnCreatedResponse()
    {
        // Arrange
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Folders,
            MockDataContextFactory.CreateMockDbSet([_parentFolder]));

        mockDataContext.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(_user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFolderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFolderQueryResponse(_parentFolder));

        var command = new CreateFolderCommand { UserName = _user.UserName, Name = "NewFolderName", ParentId = _parentFolder.Id };
        var handler = new CreateFolderCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Created);
        response.Folder.Should().NotBeNull();
        response.Folder.Name.Should().Be("NewFolderName");
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
            .ReturnsAsync(new GetUserQueryResponse(Status.NotFound, new RequestFailure { Exception = new EntityNotFoundException(_user.Id, nameof(User)), UserFriendlyMessage = Translations.RequestStatuses.NotFound }));

        var command = new CreateFolderCommand { UserName = _user.UserName, Name = "NewFolder", ParentId = _parentFolder.Id };
        var handler = new CreateFolderCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.NotFound);
        response.Folder.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenParentFolderNotFound_ThenReturnNotFoundResponse()
    {
        // Arrange
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Folders,
            MockDataContextFactory.CreateMockDbSet(Enumerable.Empty<Folder>()));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(_user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFolderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFolderQueryResponse(Status.NotFound, new RequestFailure { Exception = new EntityNotFoundException(_user.Id, nameof(Folder)), UserFriendlyMessage = Translations.RequestStatuses.NotFound }));

        var command = new CreateFolderCommand { UserName = _user.UserName, Name = "NewFolder", ParentId = _parentFolder.Id };
        var handler = new CreateFolderCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.NotFound);
        response.Folder.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenUnauthorizedAccess_ThenReturnForbiddenResponse()
    {
        // Arrange
        var parentFolder = new Folder { Id = _parentFolder.Id, UserId = Guid.NewGuid().ToString()};

        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Folders,
            MockDataContextFactory.CreateMockDbSet([parentFolder]));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(_user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFolderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFolderQueryResponse(parentFolder));

        var command = new CreateFolderCommand { UserName = _user.UserName, Name = "NewFolder", ParentId = parentFolder.Id };
        var handler = new CreateFolderCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Unauthorized);
        response.Folder.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNull();
    }
}
