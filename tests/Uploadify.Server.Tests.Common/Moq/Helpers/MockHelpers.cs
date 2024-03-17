using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Uploadify.Server.Tests.Common.Moq.Helpers;

public class MockHelpers
{
    public static IFormFile CreateMockFormFile(string fileName, string contentType, string content)
    {
        var mockFormFile = new Mock<IFormFile>();
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream);

        writer.Write(content);
        writer.Flush();
        memoryStream.Position = 0;

        var contentDisposition = $"form-data; name=\"file\"; filename=\"{fileName}\"";

        mockFormFile.Setup(file => file.FileName).Returns(fileName);
        mockFormFile.Setup(file => file.Length).Returns(memoryStream.Length);
        mockFormFile.Setup(file => file.ContentType).Returns(contentType);
        mockFormFile.Setup(f => f.ContentDisposition).Returns(contentDisposition);
        mockFormFile.Setup(file => file.OpenReadStream()).Returns(memoryStream);
        mockFormFile.Setup(file => file.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, token) => memoryStream.CopyTo(stream))
            .Returns(Task.CompletedTask);

        return mockFormFile.Object;
    }
}
