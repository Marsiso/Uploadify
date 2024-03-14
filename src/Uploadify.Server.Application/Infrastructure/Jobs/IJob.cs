using Hangfire.Server;

namespace Uploadify.Server.Application.Infrastructure.Jobs;

public interface IJob
{
    public Task Execute();
}
