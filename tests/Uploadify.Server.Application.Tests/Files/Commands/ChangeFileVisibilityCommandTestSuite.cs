using FluentAssertions;
using Mapster;
using MediatR;
using Moq;
using Uploadify.Server.Application.Files.Commands;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Core.Files.Queries;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Tests.Common.Moq.Helpers;
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Application.Tests.Files.Commands;

public class ChangeFileVisibilityCommandTestSuite
{
    private readonly User _user;
    private readonly File _file;

    public ChangeFileVisibilityCommandTestSuite()
    {
        _user = new() { Id = Guid.NewGuid().ToString(), UserName = "TestUser" };
        _file = new() { Id = 1, Folder = new() { UserId = _user.Id }, UnsafeName = "FileName", IsPublic = false };
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ThenReturnSuccessResponse()
    {
        // Arrange
        var file = _file.Adapt<File>();
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Files,
            MockDataContextFactory.CreateMockDbSet([_file]));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(_user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFileQueryResponse(file));

        mockDataContext.Setup(context => context.UpdateEntity(It.IsAny<File>(), It.IsAny<CancellationToken>()));

        var command = new ChangeFileVisibilityCommand { UserName = _user.Id, FileId = file.Id, Visibility = true };
        var handler = new ChangeFileVisibilityCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.Failure.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenUnauthorizedAccess_ThenReturnUnauthorizedResponse()
    {
        // Arrange
        var file = _file.Adapt<File>();
        file.Folder.UserId = Guid.NewGuid().ToString();

        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Files,
            MockDataContextFactory.CreateMockDbSet([_file]));

        var mockSender = new Mock<ISender>();
        mockSender.Setup(sender => sender.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserQueryResponse(_user));

        mockSender.Setup(sender => sender.Send(It.IsAny<GetFileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFileQueryResponse(file));

        mockDataContext.Setup(context => context.UpdateEntity(It.IsAny<File>(), It.IsAny<CancellationToken>()));

        var command = new ChangeFileVisibilityCommand { UserName = _user.Id, FileId = file.Id, Visibility = true };
        var handler = new ChangeFileVisibilityCommandHandler(mockDataContext.Object, mockSender.Object);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Unauthorized);
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }
}
