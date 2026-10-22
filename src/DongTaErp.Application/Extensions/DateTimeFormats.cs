namespace DongTaErp.Application.Extensions;
using System.Globalization;

public static class DateFormats
{

    public static string ToDateString(this DateTime value, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        return value.ToString("d", culture);
    }
    public static string ToDateString(this DateTime? value, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        return value.HasValue ? value.Value.ToDateString(culture):"";
    }
    public static string ToDateTimeString(this DateTime value, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        return value.ToString("G", culture);
    }
    public static string ToDateTimeString(this DateTime? value, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        return value.HasValue ? value.Value.ToDateTimeString(culture) : "";
    }
}