
using Talaby.Application.Common.Interfaces;

namespace Talaby.Infrastructure.Startup;

public class EnsureStorageFoldersTask(IStorageService storageService) : IStartupTask
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await storageService.EnsureImageDirectoryExistsAsync(cancellationToken);
        await storageService.EnsureFileDirectoryExistsAsync(cancellationToken);
    }
}
