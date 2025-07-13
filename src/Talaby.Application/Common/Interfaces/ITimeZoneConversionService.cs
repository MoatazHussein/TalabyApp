namespace Talaby.Application.Common.Interfaces;

public interface ITimeZoneConversionService
{
    T ConvertUtcToLocal<T>(T dto, string? timeZoneId = null);
    List<T> ConvertListUtcToLocal<T>(List<T> list, string? timeZoneId = null);
    PagedResult<T> ConvertPagedUtcToLocal<T>(PagedResult<T> paged, string? timeZoneId = null);
}
