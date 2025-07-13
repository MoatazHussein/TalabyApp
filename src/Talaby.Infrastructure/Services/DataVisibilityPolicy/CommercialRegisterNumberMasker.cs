using Talaby.Application.Common.Interfaces;

namespace Talaby.Infrastructure.Services.DataVisibilityPolicy;

public class CommercialRegisterNumberMasker : ICommercialRegisterNumberMasker
{
    public string Mask(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return value.Length > 4
            ? value.Substring(0, 4) + new string('*', value.Length - 4)
            : new string('*', value.Length);
    }
}
