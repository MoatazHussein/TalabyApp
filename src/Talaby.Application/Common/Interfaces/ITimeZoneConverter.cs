namespace Talaby.Application.Common.Interfaces;

public interface ITimeZoneConverter
{
    T ConvertUtcToLocal<T>(T dto);
}
