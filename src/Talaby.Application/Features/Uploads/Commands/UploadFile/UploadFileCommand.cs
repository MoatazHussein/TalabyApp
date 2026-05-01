using MediatR;
using Microsoft.AspNetCore.Http;

namespace Talaby.Application.Features.Uploads.Commands.UploadFile;

public sealed record UploadFileCommand(IFormFile? File) : IRequest<string>;
