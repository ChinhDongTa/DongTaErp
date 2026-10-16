using System.Globalization;

namespace DongTaErp.Application.Extensions;

public static class DecimalFormats
{
    public const string Trimmed = "#,##0.##";

    public static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("vi-VN");

    public static string ToDisplay(this decimal value) => value.ToString(Trimmed, Culture);

    public static string ToDisplay(this decimal? value) => value?.ToDisplay() ?? string.Empty;


    public static string ToDisplay(this decimal value, string unit) => $"{value.ToString(Trimmed, Culture)} {unit}";

    public static string ToDisplay(this decimal? value, string unit) => value?.ToDisplay(unit) ?? string.Empty;
}
