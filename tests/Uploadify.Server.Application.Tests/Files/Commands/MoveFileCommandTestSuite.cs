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
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Application.Tests.Files.Commands;

public class MoveFileCommandTestSuite
{
    private readonly User _user;
    private readonly File _file;
    private readonly Folder _destinationFolder;

    public MoveFileCommandTestSuite()
    {
        _user = new User { Id = Guid.NewGuid().ToString(), UserName = "TestUser" };
        _file = new File { Id = 1, FolderId = 2, UnsafeName = "TestFile.txt", Folder = new Folder { UserId = _user.Id } };
        _destinationFolder = new Folder { Id = 3, UserId = _user.Id };
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ThenReturnSuccessResponse()
    {
        // Arrange
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Files,
            MockDataContextFactory.CreateMockDbSet([_file]));

        mockDataContext.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(_user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFileQueryResponse(_file));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFolderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFolderQueryResponse(_destinationFolder));

        var command = new MoveFileCommand { UserName = _user.UserName, FileId = _file.Id, DestinationFolderId = _destinationFolder.Id };
        var handler = new MoveFileCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.Failure.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThenReturnNotFoundResponse()
    {
        // Arrange
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Files,
            MockDataContextFactory.CreateMockDbSet([_file]));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(Status.NotFound, new RequestFailure { UserFriendlyMessage = Translations.RequestStatuses.NotFound, Exception = new EntityNotFoundException(_user.Id, nameof(User)) }));

        var command = new MoveFileCommand { UserName = _user.UserName, FileId = _file.Id };
        var handler = new MoveFileCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Status.Should().Be(Status.NotFound);
        response.File.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_WhenFileNotFound_ThenReturnNotFoundResponse()
    {
        // Arrange
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Files,
            MockDataContextFactory.CreateMockDbSet(Enumerable.Empty<File>()));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(_user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFileQueryResponse(Status.NotFound, new RequestFailure { UserFriendlyMessage = Translations.RequestStatuses.NotFound, Exception = new EntityNotFoundException(_file.Id.ToString(), nameof(File)) }));

        var command = new MoveFileCommand { UserName = _user.UserName, FileId = 1 };
        var handler = new MoveFileCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Status.Should().Be(Status.NotFound);
        response.File.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_WhenUnauthorizedAccess_ThenReturnUnauthorizedResponse()
    {
        // Arrange
        var file = new File { Id = _file.Id, FolderId = _file.FolderId, UnsafeName = _file.UnsafeName, Folder = new Folder { UserId = Guid.NewGuid().ToString()} };

        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Files,
            MockDataContextFactory.CreateMockDbSet(new List<File> { file }));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(_user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFileQueryResponse(file));

        var command = new MoveFileCommand { UserName = _user.UserName, FileId = 1 };
        var handler = new MoveFileCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Status.Should().Be(Status.Unauthorized);
        response.File.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_WhenDestinationFolderNotFound_ThenReturnNotFoundResponse()
    {
        // Arrange
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Files,
            MockDataContextFactory.CreateMockDbSet([_file]));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(_user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFileQueryResponse(_file));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFolderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFolderQueryResponse(Status.NotFound, new RequestFailure { UserFriendlyMessage = Translations.RequestStatuses.NotFound, Exception = new EntityNotFoundException("3", nameof(Folder)) }));

        var command = new MoveFileCommand { UserName = _user.UserName, FileId = _file.Id, DestinationFolderId = 3 };
        var handler = new MoveFileCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Status.Should().Be(Status.NotFound);
        response.File.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }
}
