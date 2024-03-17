using FluentAssertions;
using Uploadify.Server.Core.Files.Queries;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Tests.Common.Moq.Helpers;
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Core.Tests.Files.Queries;

public class GetFileQueryTestSuite
{
    [Fact]
    public async Task Handle_WhenFileExists_ReturnFile()
    {
        // Arrange
        var file = new File { Id = 1 };
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Files,
            MockDataContextFactory.CreateMockDbSet(new List<File>
            {
                file,
                new() { Id = 2 },
                new() { Id = 3 }
            }.AsQueryable()));

        var query = new GetFileQuery { FileId = file.Id };
        var handler = new GetFileQueryHandler(mockDataContext.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.File.Should().NotBeNull();
        response.File.Should().BeEquivalentTo(file);
        response.File.Id.Should().Be(file.Id);
        response.Failure.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenFileDoesNotExist_ReturnFailure()
    {
        // Arrange
        var file = new File { Id = 1 };
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Files,
            MockDataContextFactory.CreateMockDbSet(new List<File>
            {
                file,
                new() { Id = 2 },
                new() { Id = 3 }
            }.AsQueryable()));

        var query = new GetFileQuery { FileId = 4 };
        var handler = new GetFileQueryHandler(mockDataContext.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.NotFound);
        response.File.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }
}
