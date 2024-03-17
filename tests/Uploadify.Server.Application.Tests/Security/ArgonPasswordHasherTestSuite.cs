using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Uploadify.Server.Application.Security.Services;
using Uploadify.Server.Domain.Application.Models;

namespace Uploadify.Server.Application.Tests.Security;

public class ArgonPasswordHasherTestSuite
{
    [Theory]
    [InlineData("SamplePassword1234**")]
    [InlineData("S")]
    [InlineData("SamplePassword1234**SamplePassword1234**SamplePassword1234**SamplePassword1234**SamplePassword1234**SamplePassword1234**")]
    public void HashPassword_WhenValidInput_ThenReturnValue(string password)
    {
        // Arrange
        var options = Options.Create(new ArgonPasswordHasherOptions());
        var hasher = new ArgonPasswordHasher<User>(options) as IPasswordHasher<User>;

        var user = new User();

        // Act
        // var exception = Record.Exception(() => hasher.HashPassword(user, password));
        var hash = hasher.HashPassword(user, password);

        // Assert
        hash.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void HashPassword_WhenInvalidInput_ThenThrowException(string password)
    {
        // Arrange
        var options = Options.Create(new ArgonPasswordHasherOptions());
        var hasher = new ArgonPasswordHasher<User>(options) as IPasswordHasher<User>;

        var user = new User();

        // Act
        var exception = Record.Exception(() => hasher.HashPassword(user, password));

        // Assert
        exception.Should().NotBeNull();
    }

    [Theory]
    [InlineData("SamplePassword1234**")]
    [InlineData("S")]
    [InlineData("SamplePassword1234**SamplePassword1234**SamplePassword1234**SamplePassword1234**SamplePassword1234**SamplePassword1234**")]
    public void VerifyHashedPassword_WhenValidInput_ThenReturnSuccessStatus(string password)
    {
        // Arrange
        var options = Options.Create(new ArgonPasswordHasherOptions());
        var hasher = new ArgonPasswordHasher<User>(options) as IPasswordHasher<User>;

        var user = new User();

        // Act
        var hash = hasher.HashPassword(user, password);
        var result = hasher.VerifyHashedPassword(user, hash, password);

        // Assert
        hash.Should().NotBeNullOrWhiteSpace();
        result.Should().Be(PasswordVerificationResult.Success);
    }

    [Theory]
    [InlineData("SamplePassword1234**", "samplepassword1234**")]
    [InlineData("SamplePassword1234**", "SamplePassword**")]
    [InlineData("SamplePassword1234**", "SamplePassword1234")]
    public void VerifyHashedPassword_WhenInvalidInput_ThenReturnFailStatus(string originalPassword, string password)
    {
        // Arrange
        var options = Options.Create(new ArgonPasswordHasherOptions());
        var hasher = new ArgonPasswordHasher<User>(options) as IPasswordHasher<User>;

        var user = new User();

        // Act
        var hash = hasher.HashPassword(user, originalPassword);
        var result = hasher.VerifyHashedPassword(user, hash, password);

        // Assert
        hash.Should().NotBeNullOrWhiteSpace();
        result.Should().Be(PasswordVerificationResult.Failed);
    }


    [Theory]
    [InlineData(null, "SamplePassword1234**")]
    [InlineData("", "SamplePassword1234**")]
    [InlineData("9mBvHzHd47verYcVDkAqTBeqHs9rf2PcM9/mJWd+jAA=.DeXzCnFAK4NBVrL7PHyhIA==", null)]
    [InlineData("9mBvHzHd47verYcVDkAqTBeqHs9rf2PcM9/mJWd+jAA=.DeXzCnFAK4NBVrL7PHyhIA==", "")]
    public void VerifyHashedPassword_WhenInvalidInput_ThenThrowException(string hash, string password)
    {
        // Arrange
        var options = Options.Create(new ArgonPasswordHasherOptions());
        var hasher = new ArgonPasswordHasher<User>(options) as IPasswordHasher<User>;

        var user = new User();

        // Act
        var exception = Record.Exception(() => hasher.VerifyHashedPassword(user, hash, password));

        // Assert
        exception.Should().NotBeNull();
    }
}
