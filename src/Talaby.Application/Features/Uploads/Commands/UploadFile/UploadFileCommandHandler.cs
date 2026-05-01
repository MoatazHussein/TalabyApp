using MediatR;
using Talaby.Application.Common.Interfaces;

namespace Talaby.Application.Features.Uploads.Commands.UploadFile;

public sealed class UploadFileCommandHandler(IStorageService storageService)
    : IRequestHandler<UploadFileCommand, string>
{
    public async Task<string> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        return await storageService.SaveFileAsync(request.File!, cancellationToken);
    }
}
