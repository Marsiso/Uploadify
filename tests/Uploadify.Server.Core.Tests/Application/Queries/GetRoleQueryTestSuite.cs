using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Tests.Common.Moq.Helpers;

namespace Uploadify.Server.Core.Tests.Application.Queries;

public class GetRoleQueryTestSuite
{
    [Fact]
    public async Task Handle_WhenRoleExists_ThenReturnSuccessResponse()
    {
        // Arrange
        const string roleName = "admin";
        const string normalizedRoleName = "ADMIN";

        var mockNormalizer = new Mock<ILookupNormalizer>();

        mockNormalizer.Setup(lookupNormalizer => lookupNormalizer.NormalizeName(roleName)).Returns(normalizedRoleName);

        var role = new Role { Name = roleName, NormalizedName = normalizedRoleName };
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Roles,
            MockDataContextFactory.CreateMockDbSet(new List<Role> { role }.AsQueryable()));

        var query = new GetRoleQuery(roleName);
        var handler = new GetRoleQueryHandler(mockDataContext.Object, mockNormalizer.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.Role.Should().NotBeNull();
        response.Role.Name.Should().NotBeNullOrWhiteSpace();
        response.Role.NormalizedName.Should().NotBeNullOrWhiteSpace();
        response.Failure.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenRoleDoesNotExist_ThenReturnNotFoundResponse()
    {
        // Arrange
        const string roleName = "admin";
        const string normalizedRoleName = "ADMIN";

        var mockNormalizer = new Mock<ILookupNormalizer>();

        mockNormalizer.Setup(lookupNormalizer => lookupNormalizer.NormalizeName(roleName)).Returns(normalizedRoleName);

        var role = new Role { Name = roleName, NormalizedName = normalizedRoleName };
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Roles,
            MockDataContextFactory.CreateMockDbSet(new List<Role> { role }.AsQueryable()));

        var query = new GetRoleQuery($"{roleName}{roleName}");
        var handler = new GetRoleQueryHandler(mockDataContext.Object, mockNormalizer.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.NotFound);
        response.Role.Should().BeNull();
        response.Failure.Should().NotBeNull();
        response.Failure.Exception.Should().NotBeNull();
        response.Failure.UserFriendlyMessage.Should().NotBeNullOrWhiteSpace();
    }
}
