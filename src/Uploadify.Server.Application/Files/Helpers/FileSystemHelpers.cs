using CommunityToolkit.Diagnostics;
using Microsoft.Net.Http.Headers;
using static System.String;

namespace Uploadify.Server.Application.Files.Helpers;

public static class FileSystemHelpers
{
    public const int StaticFilesDirectoryMaximumFiles = 4_096;
    public const string StaticFilesFolderName = "Files";

    public static string GetFileName(string headerValue)
    {
        Guard.IsNotNullOrWhiteSpace(headerValue);

        var filename = ContentDispositionHeaderValue.Parse(headerValue).FileName.Trim();
        if (!filename.HasValue)
        {
            throw new InvalidOperationException();
        }

        return filename.Value;
    }

    public static string GetFilePathBase()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), StaticFilesFolderName);
    }

    public static string GetFilePath(string filename)
    {
        Guard.IsNotNullOrWhiteSpace(filename);

        var pathBase = GetFilePathBase();

        string? path = null;
        foreach (var directory in Directory.EnumerateDirectories(pathBase))
        {
            path = Path.Combine(pathBase, directory);
            if (Directory.EnumerateFileSystemEntries(path).Count() > StaticFilesDirectoryMaximumFiles)
            {
                path = null;
                continue;
            }

            break;
        }

        if (IsNullOrWhiteSpace(path))
        {
            string directory;
            do
            {
                directory = Guid.NewGuid().ToString();
                path = Path.Combine(pathBase, directory);
            } while (Directory.Exists(Path.Combine(pathBase, directory)));

            Directory.CreateDirectory(path);
        }


        return Path.Combine(path, filename);
    }
}
