using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Tests.Common.Moq.Helpers;

namespace Uploadify.Server.Core.Tests.Application.Queries;

public class UniqueEmailQueryTestSuite
{
    [Fact]
    public async Task Handle_WhenEmailExists_ThenReturnFailureValidationResponse()
    {
        // Arrange
        const string email = "test.user@prov.dev";
        const string normalizedEmail = "TEST.USER@PROV.DEV";

        var mockNormalizer = new Mock<ILookupNormalizer>();

        mockNormalizer.Setup(lookupNormalizer => lookupNormalizer.NormalizeEmail(email)).Returns(normalizedEmail);

        var user = new User { Email = email, NormalizedEmail = normalizedEmail };
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Users,
            MockDataContextFactory.CreateMockDbSet(new List<User> { user }.AsQueryable()));

        var query = new UniqueEmailQuery(email);
        var handler = new UniqueEmailQueryHandler(mockDataContext.Object, mockNormalizer.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.IsUnique.Should().BeFalse();
        response.Failure.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ThenReturnsSuccessValidationResponse()
    {
        // Arrange
        const string email = "test.user@prov.dev";
        const string normalizedEmail = "TEST.USER@PROV.DEV";

        var mockNormalizer = new Mock<ILookupNormalizer>();

        mockNormalizer.Setup(lookupNormalizer => lookupNormalizer.NormalizeEmail(email)).Returns(normalizedEmail);

        var user = new User { Email = email, NormalizedEmail = normalizedEmail };
        var mockDataContext = MockDataContextFactory.SetupDataContext(
            context => context.Users,
            MockDataContextFactory.CreateMockDbSet(new List<User> { user }.AsQueryable()));

        var query = new UniqueEmailQuery("second.test.user@prov.dev");
        var handler = new UniqueEmailQueryHandler(mockDataContext.Object, mockNormalizer.Object);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(Status.Ok);
        response.IsUnique.Should().BeTrue();
        response.Failure.Should().BeNull();
    }
}
