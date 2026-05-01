using Microsoft.AspNetCore.Http;
using Talaby.Application.Common.Interfaces;

namespace Talaby.Infrastructure.Services.FileStorage;

public class StorageService : IStorageService
{
    private const string StorageDirectory = "Storage";
    private const string ImagesDirectory = "Images";
    private const string FilesDirectory = "Files";

    private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), StorageDirectory);

    public Task EnsureImageDirectoryExistsAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(GetPhysicalDirectoryPath(ImagesDirectory));
        return Task.CompletedTask;
    }

    public async Task<string> SaveImageAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await EnsureImageDirectoryExistsAsync(cancellationToken);
        return await SaveAsync(file, ImagesDirectory, cancellationToken);
    }

    public Task EnsureFileDirectoryExistsAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(GetPhysicalDirectoryPath(FilesDirectory));
        return Task.CompletedTask;
    }

    public async Task<string> SaveFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await EnsureFileDirectoryExistsAsync(cancellationToken);
        return await SaveAsync(file, FilesDirectory, cancellationToken);
    }

    private async Task<string> SaveAsync(
        IFormFile file,
        string directoryName,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var physicalPath = Path.Combine(GetPhysicalDirectoryPath(directoryName), fileName);

        await using var stream = File.Create(physicalPath);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/{StorageDirectory}/{directoryName}/{fileName}";
    }

    private string GetPhysicalDirectoryPath(string directoryName)
        => Path.Combine(_storagePath, directoryName);
}
