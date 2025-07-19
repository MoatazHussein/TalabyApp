using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Talaby.Application.Common.Interfaces;

namespace Talaby.Infrastructure.Services.TimeConversion;

public class TimeZoneConverter : ITimeZoneConverter
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _defaultTimeZone;
    private readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();

    public TimeZoneConverter(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _defaultTimeZone = configuration["AppSettings:DefaultTimeZone"] ?? "Arab Standard Time";
    }

    public T ConvertUtcToLocal<T>(T dto)
    {
        if (dto == null) return default!;
        ConvertObject(dto!, GetTimeZoneId());
        return dto;
    }

    private void ConvertObject(object obj, string timeZoneId)
    {
        var type = obj.GetType();

        if (obj is IEnumerable<object> enumerable)
        {
            foreach (var item in enumerable)
                ConvertObject(item, timeZoneId);
            return;
        }

        var props = _propertyCache.GetOrAdd(type, t =>
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
             .Where(p => p.CanRead && p.CanWrite)
             .ToArray());

        foreach (var prop in props)
        {
            var value = prop.GetValue(obj);
            if (value == null) continue;

            if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
            {
                var date = (DateTime?)value;
                if (date.HasValue)
                    prop.SetValue(obj, ConvertUtcToLocalDate(date.Value, timeZoneId));
            }
            else if (!prop.PropertyType.IsPrimitive && prop.PropertyType != typeof(string))
            {
                ConvertObject(value, timeZoneId);
            }
        }
    }
    private DateTime ConvertUtcToLocalDate(DateTime utcDate, string timeZoneId)
    {
        if (utcDate.Kind != DateTimeKind.Utc)
            utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);

        var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(utcDate, tz);
    }

    private string GetTimeZoneId()
    {
        var timeZoneId = _httpContextAccessor.HttpContext?.Request.Headers["X-Timezone-Id"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(timeZoneId) || !IsValidTimeZone(timeZoneId))
            return _defaultTimeZone;

        return timeZoneId!;
    }

    private bool IsValidTimeZone(string? id)
    {
        return !string.IsNullOrWhiteSpace(id) &&
               TimeZoneInfo.GetSystemTimeZones().Any(z => z.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }
}