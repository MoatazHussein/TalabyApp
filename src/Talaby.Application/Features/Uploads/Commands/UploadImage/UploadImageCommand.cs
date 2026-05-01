using MediatR;
using Microsoft.AspNetCore.Http;

namespace Talaby.Application.Features.Uploads.Commands.UploadImage;

public sealed record UploadImageCommand(IFormFile? File) : IRequest<string>;
