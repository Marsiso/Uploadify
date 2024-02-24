using Bogus;
using Uploadify.Server.Domain.FileSystem.Models;

namespace Uploadify.Server.Application.FileSystem.Generators;

internal static class FolderGenerator
{
    internal static Faker<Folder> GetGenerator(string userId, int? parentId)
    {
        return new Faker<Folder>()
            .RuleFor(folder => folder.UserId, () => userId)
            .RuleFor(folder => folder.ParentId, () => parentId)
            // .RuleFor(folder => folder.Name, faker => faker.System.FileName())
            .RuleFor(folder => folder.Name, faker => faker.Commerce.ProductName());
    }

    internal static IEnumerable<Folder> GenerateFolders(int count, string userId, int? parentId = null)
    {
        return GetGenerator(userId, parentId).Generate(count)
            .DistinctBy(folder => new { folder.ParentId, folder.Name })
            .ToList();
    }
}
