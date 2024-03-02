using Bogus;
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Application.Files.Generators;

internal static class FileGenerator
{
    internal static Faker<File> GetGenerator(int folderId)
    {
        return new Faker<File>()
            .RuleFor(file => file.FolderId, () => folderId)
            .RuleFor(file => file.MimeType, faker => faker.System.MimeType())
            .RuleFor(file => file.Extension, (faker, file) => faker.System.FileExt(file.MimeType))
            .RuleFor(file => file.UnsafeName, (faker, file) => faker.System.FileName(file.Extension))
            .RuleFor(file => file.SafeName, (_, file) => $"{Guid.NewGuid().ToString()}.{file.Extension}")
            .RuleFor(file => file.Location, (faker, file) => Path.Combine(faker.System.DirectoryPath(), file.SafeName))
            .RuleFor(file => file.Size, faker => faker.Random.Long(10L, 32L));
    }

    internal static IEnumerable<File> GenerateFiles(int count, int folderId)
    {
        return GetGenerator(folderId).Generate(count)
            .DistinctBy(file => file.UnsafeName)
            .ToList();
    }
}
