namespace DongTaErp.Application.Extensions;

public static class SafeComparisonExtensions
{
    // =====================================================
    // 1. Generic - Dành cho các kiểu Nullable (DateOnly?, int?, Guid?, bool?, ...)
    // =====================================================
    /// <summary>
    /// So sánh an toàn cho các kiểu Nullable.
    /// Trả về true nếu giá trị mới khác giá trị hiện tại (hỗ trợ cả việc set về null).
    /// </summary>
    public static bool IsDifferentFrom<T>(this T? newValue, T? currentValue)
    {
        return !EqualityComparer<T>.Default.Equals(newValue, currentValue);
    }

    //======================================================
    // 2. Generic - Dành cho các kiểu Nullable với giá trị không null (DateOnly?, int?, Guid?, bool?, ...)
    //======================================================
    /// <summary>
    /// So sánh an toàn cho các kiểu Nullable với giá trị không null.
    /// Trả về true nếu giá trị mới khác giá trị hiện tại.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="newValue"></param>
    /// <param name="currentValue"></param>
    /// <returns></returns>
    public static bool HasValueAndIsDifferentFrom<T>(this T? newValue, T? currentValue) where T : struct
    {
        return newValue.HasValue && !Nullable.Equals(newValue, currentValue);
    }
}
