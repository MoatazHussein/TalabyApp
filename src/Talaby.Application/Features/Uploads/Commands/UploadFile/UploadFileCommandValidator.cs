using FluentValidation;

namespace Talaby.Application.Features.Uploads.Commands.UploadFile;

public sealed class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    private const long MaxFileSizeInBytes = 10 * 1024 * 1024;

    private static readonly string[] AllowedFileExtensions =
    [
        ".pdf",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".ppt",
        ".pptx",
        ".txt"
    ];

    public UploadFileCommandValidator()
    {
        RuleFor(command => command.File)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("File is required")
            .Must(file => file!.Length > 0).WithMessage("File is required")
            .Must(file => file!.Length <= MaxFileSizeInBytes).WithMessage("File size must not exceed 10 MB")
            .Must(file => AllowedFileExtensions.Contains(Path.GetExtension(file!.FileName).ToLowerInvariant()))
            .WithMessage("File extension must be one of: .pdf, .doc, .docx, .xls, .xlsx, .ppt, .pptx, .txt");
    }
}
