using System.Text;
using System.Text.RegularExpressions;

namespace DongTaErp.Application.Extensions;
public static class StringHelpers
{
    /// <summary>
    /// Chuyển danh sách chuỗi thành một dòng các item sẽ được phân biệt bằng separator.
    /// </summary>
    /// <param name="strings">Danh sách chuỗi</param>
    /// <param name="separator">Ký tự phân cách mặc định là ", "</param>
    public static string ToLine(this IEnumerable<string> strings, string separator = ", ")
        => string.Join(separator, strings);

    public static string ToVND(this decimal amount)
    {
        var viCulture = new System.Globalization.CultureInfo("vi-VN");
        return amount.ToString("N0", viCulture) + " đ";
    }

    public static string ToVND(this decimal? amount)
    {
        var viCulture = new System.Globalization.CultureInfo("vi-VN");
        return amount.HasValue ? amount.Value.ToString("N0", viCulture) + " đ" : string.Empty;
    }

    // <summary>
    /// Kiểm tra xem giá trị mới có khác với giá trị hiện tại không (đã trim và xử lý null).
    /// </summary>
    public static bool IsDifferentFrom(this string? newValue, string? currentValue)
    {
        var normalizedNew = Normalize(newValue);
        var normalizedCurrent = Normalize(currentValue);

        return normalizedNew != normalizedCurrent;
    }

    /// <summary>
    /// Dành cho string không nullable.
    /// Chỉ trả về true nếu giá trị mới có nội dung và khác giá trị hiện tại.
    /// </summary>
    public static bool HasValueAndIsDifferentFrom(this string? newValue, string currentValue)
    {
        var normalizedNew = Normalize(newValue);
        var normalizedCurrent = Normalize(currentValue) ?? string.Empty;

        return !string.IsNullOrEmpty(normalizedNew) && normalizedNew != normalizedCurrent;
    }

    /// <summary>
    /// Chuẩn hóa chuỗi: nếu null hoặc rỗng thì trả về null, nếu có giá trị thì loại bỏ khoảng trắng đầu và cuối.
    /// </summary>
    /// <param name="value">Giá trị cần chuẩn hóa</param>
    /// <returns>Giá trị đã chuẩn hóa hoặc null nếu rỗng</returns>
    public static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <summary>
    /// Sanitize tên: loại bỏ khoảng trắng, bỏ dấu tiếng Việt, loại bỏ ký tự không phải chữ cái hoặc số, chuyển sang chữ hoa.
    /// Trần-Đình/Long=>TranDinhLong, phòng 02=>Phong02
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string SanitizeName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "NA";

        // Trim và chuẩn hóa khoảng trắng
        var value = name.Trim();
        value = Regex.Replace(value, @"\s+", " ");

        // Bỏ dấu tiếng Việt
        value = RemoveDiacritics(value);

        // Thay ký tự đặc biệt bằng khoảng trắng (giữ lại chữ và số)
        value = Regex.Replace(value, @"[^A-Za-z0-9]", " ");

        // Chuẩn hóa khoảng trắng lần nữa
        value = Regex.Replace(value, @"\s+", " ");

        // Tách từ và viết hoa chữ cái đầu, giữ nguyên số
        var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();
        foreach (var w in words)
        {
            if (w.Length > 0)
            {
                if (char.IsLetter(w[0]))
                {
                    sb.Append(char.ToUpperInvariant(w[0]));
                    if (w.Length > 1)
                        sb.Append(w.Substring(1).ToLowerInvariant());
                }
                else
                {
                    // Nếu bắt đầu bằng số thì giữ nguyên
                    sb.Append(w);
                }
            }
        }

        return sb.ToString();
    }

    private static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
