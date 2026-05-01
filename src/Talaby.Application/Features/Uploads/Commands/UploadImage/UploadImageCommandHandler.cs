using MediatR;
using Talaby.Application.Common.Interfaces;

namespace Talaby.Application.Features.Uploads.Commands.UploadImage;

public sealed class UploadImageCommandHandler(IStorageService storageService)
    : IRequestHandler<UploadImageCommand, string>
{
    public async Task<string> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        return await storageService.SaveImageAsync(request.File!, cancellationToken);
    }
}
