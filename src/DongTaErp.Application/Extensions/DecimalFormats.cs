using System.Globalization;

namespace DongTaErp.Application.Extensions;

public static class DecimalFormats
{
    public const string DefaultFormat = "#,##0.##";

    public static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("vi-VN");

    public static string ToDisplay(this decimal value, string format = DefaultFormat, CultureInfo? culture = null)
        => value.ToString(format, culture ?? DefaultCulture);

    public static string ToDisplay(this decimal? value, string format = DefaultFormat, CultureInfo? culture = null)
        => value?.ToDisplay(format, culture) ?? string.Empty;

    public static string ToDisplay(
        this decimal value, string unit, string format = DefaultFormat, CultureInfo? culture = null)
        => $"{value.ToDisplay(format, culture)} {unit}";

    public static string ToCurrency(this decimal value, CultureInfo? culture = null, int decimals = 0)
        => value.ToString($"C{decimals}", culture ?? DefaultCulture);

    public static string ToCurrency(this decimal? value, CultureInfo? culture = null, int decimals = 0)
        => value?.ToCurrency(culture, decimals) ?? string.Empty;
}