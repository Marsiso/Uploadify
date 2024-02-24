using Bogus;
using CommunityToolkit.Diagnostics;
using Uploadify.Server.Application.Authentication.Commands;

namespace Uploadify.Server.Application.Application.Generators;

internal static class SignUpCommandGenerator
{
    internal static Faker<SignUpCommand> GetGenerator(string password = "Uploadify2024*")
    {
        return new Faker<SignUpCommand>()
            .RuleFor(user => user.UserName, faker => faker.Internet.UserName())
            .RuleFor(user => user.Email, faker => faker.Internet.Email().ToLower())
            .RuleFor(user => user.PhoneNumber, faker => faker.Phone.PhoneNumber("###-###-###-###"))
            .RuleFor(user => user.GivenName, faker => faker.Name.FirstName())
            .RuleFor(user => user.FamilyName, faker => faker.Name.LastName())
            .RuleFor(user => user.Password, () => password)
            .RuleFor(user => user.PasswordRepeat, (_, user) => user.Password);
    }

    internal static List<SignUpCommand> GenerateCommands(int count)
    {
        Guard.IsGreaterThanOrEqualTo(count, 0);
        return GetGenerator().Generate(count)
            .DistinctBy(user => user.UserName)
            .DistinctBy(user => user.Email)
            .ToList();
    }
}
