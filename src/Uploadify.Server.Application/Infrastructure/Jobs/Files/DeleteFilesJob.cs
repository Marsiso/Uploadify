using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Files.Models;
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Application.Infrastructure.Jobs.Files;

public class DeleteFilesJob : IJob
{
    public static Func<DataContext, IAsyncEnumerable<Folder>> DeletedFoldersQuery = EF.CompileAsyncQuery((DataContext context) =>
        context.Folders.IgnoreQueryFilters().Where(folder => !folder.IsActive && folder.DateUpdated.AddDays(7) < DateTime.UtcNow));

    public static Func<DataContext, IAsyncEnumerable<File>> DeletedFilesQuery = EF.CompileAsyncQuery((DataContext context) =>
        context.Files.IgnoreQueryFilters().Where(file => !file.IsActive && file.DateUpdated.AddDays(7) < DateTime.UtcNow));

    private readonly DataContext _context;
    private readonly ILogger<DeleteFilesJob> _logger;

    public DeleteFilesJob(DataContext context, ILogger<DeleteFilesJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Execute()
    {
        try
        {
            await DeleteFiles();
            await DeleteFolders();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            _logger.LogError($"Job: '{nameof(DeleteFilesJob)}' Action: '{nameof(Execute)}' Message: '{exception.Message}'.");
        }
    }

    private async Task DeleteFiles()
    {
        var files = new List<File>();
        await foreach (var file in DeletedFilesQuery(_context))
        {
            files.Add(file);
        }

        foreach (var file in files)
        {
            var executionStrategy = _context.Database.CreateExecutionStrategy();

            await ExecutionStrategyExtensions.ExecuteAsync(executionStrategy, async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(default);

                try
                {
                    if (!System.IO.File.Exists(file.Location))
                    {
                        return;
                    }

                    System.IO.File.Delete(file.Location);

                    await _context.DeleteEntity(file);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    _logger.LogInformation($"Job: '{nameof(DeleteFilesJob)}' Action: '{nameof(DeleteFiles)}' Message: 'File {file.UnsafeName}' successfully deleted.'.");
                }
                catch (Exception exception)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"Job: '{nameof(DeleteFilesJob)}' Action: '{nameof(DeleteFiles)}' Message: 'File {file.UnsafeName} deletion failure. {exception.Message}'.");
                }
            }).ConfigureAwait(false);
        }
    }

    private async Task DeleteFolders()
    {
        var folders = new List<Folder>();
        await foreach (var folder in DeletedFoldersQuery(_context))
        {
            folders.Add(folder);
        }

        if (folders.Count == 0)
        {
            return;
        }

        var executionStrategy = _context.Database.CreateExecutionStrategy();

        await ExecutionStrategyExtensions.ExecuteAsync(executionStrategy, async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(default);

            try
            {
                _context.RemoveRange(folders);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Job: '{nameof(DeleteFilesJob)}' Action: '{nameof(DeleteFolders)}' Message: 'Successfully deleted {folders.Count} folders.'.");
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Job: '{nameof(DeleteFilesJob)}' Action: '{nameof(DeleteFolders)}' Message: 'Folders deletion failure. {exception.Message}'.");
            }
        }).ConfigureAwait(false);
    }
}
