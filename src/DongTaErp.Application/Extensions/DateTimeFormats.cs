namespace DongTaErp.Application.Extensions;

public static class DateTimeFormats
{
    public static string ToVnDate(this DateTime dateTime, char separator='/')
    {
        return $"{dateTime.Day}{separator}{dateTime.Month}{separator}{dateTime.Year}";
    }
    public static string ToVnDateTime(this DateTime dateTime, char separator = '/')
    {
        return $"{dateTime.Day}{separator}{dateTime.Month}{separator}{dateTime.Year} {dateTime.Hour}:{dateTime.Minute}:{dateTime.Millisecond}";
    }
    public static string ToVnDate(this DateTime? dateTime, char separator = '/')
    {
        return dateTime.HasValue ? dateTime.Value.ToVnDate(separator) : "";
    }
    public static string ToVnDateTime(this DateTime? dateTime, char separator = '/')
    {
        return dateTime.HasValue ? dateTime.Value.ToVnDateTime(separator) : "";
    }
}