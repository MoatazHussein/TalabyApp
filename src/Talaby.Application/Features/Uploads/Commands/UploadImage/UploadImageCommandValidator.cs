using FluentValidation;

namespace Talaby.Application.Features.Uploads.Commands.UploadImage;

public sealed class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
{
    private const long MaxImageSizeInBytes = 5 * 1024 * 1024;

    private static readonly string[] AllowedImageExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    ];

    public UploadImageCommandValidator()
    {
        RuleFor(command => command.File)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Image is required")
            .Must(file => file!.Length > 0).WithMessage("Image is required")
            .Must(file => file!.Length <= MaxImageSizeInBytes).WithMessage("Image size must not exceed 5 MB")
            .Must(file => file!.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Only image uploads are allowed")
            .Must(file => AllowedImageExtensions.Contains(Path.GetExtension(file!.FileName).ToLowerInvariant()))
            .WithMessage("Image extension must be one of: .jpg, .jpeg, .png, .webp");
    }
}
