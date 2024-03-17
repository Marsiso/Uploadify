using FluentAssertions;
using Mapster;
using MediatR;
using Moq;
using Uploadify.Server.Application.Files.Commands;
using Uploadify.Server.Application.Files.Models;
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

public class DeleteFileCommandTestSuite
{
    private readonly User _user;
    private readonly Folder _folder;
    private readonly File _file;

    public DeleteFileCommandTestSuite()
    {
        _user = new User { Id = Guid.NewGuid().ToString(), UserName = "TestUser" };
        _folder = new Folder { UserId = _user.Id };
        _file = new File { Id = 1, Folder = _folder };
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

        var command = new DeleteFileCommand { UserName = _user.UserName, FileId = _file.Id };
        var handler = new DeleteFileCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.File.Should().BeEquivalentTo(_file.Adapt<FileOverview>());
        response.Failure.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThenReturnNotFoundResponse()
    {
        // Arrange
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Files,
            MockDataContextFactory.CreateMockDbSet(Enumerable.Empty<File>()));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(Status.NotFound, new RequestFailure { UserFriendlyMessage = Translations.RequestStatuses.NotFound, Exception = new EntityNotFoundException(_user.Id, nameof(User)) }));

        var command = new DeleteFileCommand { UserName = _user.UserName, FileId = _file.Id };
        var handler = new DeleteFileCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
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
            .ReturnsAsync(new GetFileQueryResponse(Status.NotFound,  new RequestFailure { UserFriendlyMessage = Translations.RequestStatuses.NotFound, Exception = new EntityNotFoundException(_file.Id.ToString(), nameof(File)) }));

        var command = new DeleteFileCommand { UserName = _user.UserName, FileId = _file.Id };
        var handler = new DeleteFileCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
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
        var folder = new Folder { UserId = Guid.NewGuid().ToString() };
        var file = new File { Id = _file.Id, Folder = folder };

        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Files,
            MockDataContextFactory.CreateMockDbSet([file]));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(_user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFileQueryResponse(file));

        var command = new DeleteFileCommand { UserName = _user.UserName, FileId = file.Id };
        var handler = new DeleteFileCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Unauthorized);
        response.File.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }
}
