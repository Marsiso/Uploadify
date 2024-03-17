using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Tests.Common.Moq.Helpers;

namespace Uploadify.Server.Core.Tests.Application.Queries;

public class GetUserQueryTestSuite
{
    [Fact]
    public async Task Handle_WhenUserExists_ThenReturnsSuccessResponse()
    {
        // Arrange
        const string userName = "TestUser";
        const string normalizedUserName = "TESTUSER";

        var mockNormalizer = new Mock<ILookupNormalizer>();

        mockNormalizer.Setup(lookupNormalizer => lookupNormalizer.NormalizeName(userName)).Returns(normalizedUserName);

        var user = new User { UserName = userName, NormalizedUserName = normalizedUserName };
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Users,
            MockDataContextFactory.CreateMockDbSet(new List<User>
            {
                user,
                new() { UserName = $"{userName}{userName}", NormalizedUserName = $"{normalizedUserName}{normalizedUserName}" },
                new() { UserName = $"{userName}{userName}{userName}", NormalizedUserName = $"{normalizedUserName}{normalizedUserName}{normalizedUserName}" },
            }.AsQueryable()));

        var query = new GetUserQuery(userName);
        var handler = new GetUserQueryHandler(mockDataContext.Object, mockNormalizer.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.User.Should().NotBeNull();
        response.User.Should().BeEquivalentTo(user);
        response.User.UserName.Should().BeEquivalentTo(user.UserName);
        response.User.NormalizedUserName.Should().BeEquivalentTo(user.NormalizedUserName);
        response.Failure.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ThenReturnSuccessResponse()
    {
        // Arrange
        const string userName = "TestUser";
        const string normalizedUserName = "TESTUSER";

        var mockNormalizer = new Mock<ILookupNormalizer>();

        mockNormalizer.Setup(lookupNormalizer => lookupNormalizer.NormalizeName(userName)).Returns(normalizedUserName);

        var user = new User { UserName = userName, NormalizedUserName = normalizedUserName };
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Users,
            MockDataContextFactory.CreateMockDbSet(new List<User>
            {
                user,
                new() { UserName = $"{userName}{userName}", NormalizedUserName = $"{normalizedUserName}{normalizedUserName}" },
                new() { UserName = $"{userName}{userName}{userName}", NormalizedUserName = $"{normalizedUserName}{normalizedUserName}{normalizedUserName}" },
            }.AsQueryable()));

        var query = new GetUserQuery($"{userName}{userName}");
        var handler = new GetUserQueryHandler(mockDataContext.Object, mockNormalizer.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.NotFound);
        response.User.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }
}
