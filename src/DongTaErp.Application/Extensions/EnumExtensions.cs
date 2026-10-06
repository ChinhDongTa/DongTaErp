using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DongTaErp.Application.Extensions;

public static class EnumExtensions
{
    public static string? ToIntString(this System.Enum? value)
        => value == null ? null : Convert.ToInt32(value).ToString();

    //public static int? ToInt(this Enum? value)
    //    => value == null ? null : Convert.ToInt32(value);
    public static int ToInt(this System.Enum value)
        => Convert.ToInt32(value);

    public static string ToDisplayName(this System.Enum value)
    {
        var type = value.GetType();
        var member = type.GetMember(value.ToString()).FirstOrDefault();
        if (member == null) return value.ToString();

        // 1. Ưu tiên [Display(Name = "...")]
        if (member.GetCustomAttribute<DisplayAttribute>() is { } display
            && display.GetName() is { } displayName)
            return displayName;

        // 2. Nếu không có thì lấy [Description("...")]
        if (member.GetCustomAttribute<DescriptionAttribute>() is { } desc
            && !string.IsNullOrEmpty(desc.Description))
            return desc.Description;

        // 3. Cuối cùng trả về tên enum (PascalCase)
        return value.ToString();
    }
}