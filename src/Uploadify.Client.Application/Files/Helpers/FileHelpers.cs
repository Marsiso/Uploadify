using static System.IO.Path;

namespace Uploadify.Client.Application.Files.Helpers;

public static class FileHelpers
{
    public static string Rename(string originalFilename, string filename)
    {
        if (!HasExtension(originalFilename))
        {
            return filename;
        }

        var extension = GetExtension(originalFilename);
        return extension.StartsWith('.') ? $"{filename}{extension}" : $"{filename}.{extension}";
    }
}
