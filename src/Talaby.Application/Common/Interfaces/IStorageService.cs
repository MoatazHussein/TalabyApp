using Microsoft.AspNetCore.Http;

namespace Talaby.Application.Common.Interfaces;

public interface IStorageService
{
    Task EnsureImageDirectoryExistsAsync(CancellationToken cancellationToken = default);
    Task<string> SaveImageAsync(IFormFile file, CancellationToken cancellationToken);

    Task EnsureFileDirectoryExistsAsync(CancellationToken cancellationToken = default);
    Task<string> SaveFileAsync(IFormFile file, CancellationToken cancellationToken);
}
