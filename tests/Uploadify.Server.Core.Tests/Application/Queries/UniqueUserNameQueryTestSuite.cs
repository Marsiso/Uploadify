using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Tests.Common.Moq.Helpers;

namespace Uploadify.Server.Core.Tests.Application.Queries;

public class UniqueUserNameQueryTestSuite
{
[Fact]
    public async Task Handle_WhenUserNameExists_ThenReturnSuccessResponse()
    {
        // Arrange
        const string userName = "TestUser";
        const string normalizedUserName = "TESTUSER";

        var mockNormalizer = new Mock<ILookupNormalizer>();

        mockNormalizer.Setup(lookupNormalizer => lookupNormalizer.NormalizeName(userName)).Returns(normalizedUserName);

        var user = new User { UserName = userName, NormalizedUserName = normalizedUserName };
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Users,
            MockDataContextFactory.CreateMockDbSet(new List<User> { user }.AsQueryable()));

        var query = new UniqueUserNameQuery(userName);
        var handler = new UniqueUserNameQueryHandler(mockDataContext.Object, mockNormalizer.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.IsUnique.Should().BeFalse();
        response.Failure.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenUserNameDoestNotExist_ThenReturnSuccessResponse()
    {
        // Arrange
        const string userName = "TestUser";
        const string normalizedUserName = "TESTUSER";

        var mockNormalizer = new Mock<ILookupNormalizer>();

        mockNormalizer.Setup(lookupNormalizer => lookupNormalizer.NormalizeName(userName)).Returns(normalizedUserName);

        var user = new User { UserName = userName, NormalizedUserName = normalizedUserName };
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Users,
            MockDataContextFactory.CreateMockDbSet(new List<User> { user }.AsQueryable()));

        var query = new UniqueUserNameQuery($"{userName}{userName}");
        var handler = new UniqueUserNameQueryHandler(mockDataContext.Object, mockNormalizer.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.IsUnique.Should().BeTrue();
        response.Failure.Should().BeNull();
    }
}
