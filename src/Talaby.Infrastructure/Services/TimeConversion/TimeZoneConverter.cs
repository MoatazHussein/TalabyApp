using System.Reflection;
using Talaby.Application.Common;
using Talaby.Application.Common.Interfaces;

namespace Talaby.Infrastructure.Services.TimeConversion;

public class TimeZoneConverter : ITimeZoneConverter
{
    public const string DefaultTimeZoneId = "Arab Standard Time";

    public T ConvertUtcToLocal<T>(T dto, string? timeZoneId)
    {
        if (dto == null) return default!;

        var timeZone = GetSafeTimeZone(timeZoneId);

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            if (prop.PropertyType == typeof(DateTime))
            {
                var utc = (DateTime)prop.GetValue(dto)!;
                if (utc.Kind != DateTimeKind.Utc)
                    utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);

                prop.SetValue(dto, TimeZoneInfo.ConvertTimeFromUtc(utc, timeZone));
            }
            else if (prop.PropertyType == typeof(DateTime?))
            {
                var utcNullable = (DateTime?)prop.GetValue(dto);
                if (utcNullable.HasValue)
                {
                    var utc = DateTime.SpecifyKind(utcNullable.Value, DateTimeKind.Utc);
                    prop.SetValue(dto, TimeZoneInfo.ConvertTimeFromUtc(utc, timeZone));
                }
            }
        }

        return dto;
    }

    public List<T> ConvertListUtcToLocal<T>(List<T> list, string? timeZoneId)
    {
        return list.Select(item => ConvertUtcToLocal(item, timeZoneId)).ToList();
    }

    public PagedResult<T> ConvertPagedUtcToLocal<T>(PagedResult<T> paged, string? timeZoneId)
    {
        paged.Items = ConvertListUtcToLocal(paged.Items.ToList(), timeZoneId);
        return paged;
    }

    private TimeZoneInfo GetSafeTimeZone(string? timeZoneId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(timeZoneId))
                return TimeZoneInfo.FindSystemTimeZoneById(DefaultTimeZoneId);

            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch
        {
            // If invalid timezone ID passed
            return TimeZoneInfo.FindSystemTimeZoneById(DefaultTimeZoneId);
        }
    }

}
