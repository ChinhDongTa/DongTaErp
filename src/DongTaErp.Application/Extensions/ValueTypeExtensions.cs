namespace DongTaErp.Application.Extensions;

/// <summary>
/// Extension methods for comparing value types in update operations.
/// </summary>
//public static class ValueTypeExtensions
//{
//    /// <summary>
//    /// Checks if a nullable value type is different from a current value.
//    /// </summary>
//    /// <typeparam name="T">The value type to compare.</typeparam>
//    /// <param name="newValue">The new value (can be null).</param>
//    /// <param name="currentValue">The current value.</param>
//    /// <returns>True if newValue is not null AND different from currentValue.</returns>
//    public static bool HasValueAndIsDifferentFrom<T>(this T? newValue, T currentValue) where T : struct, IEquatable<T>
//    {
//        return newValue.HasValue && !newValue.Value.Equals(currentValue);
//    }

//    /// <summary>
//    /// Checks if two nullable value types are different.
//    /// Used for optional fields that can be null.
//    /// </summary>
//    /// <typeparam name="T">The value type to compare.</typeparam>
//    /// <param name="newValue">The new value (can be null).</param>
//    /// <param name="currentValue">The current value (can be null).</param>
//    /// <returns>True if the values are different.</returns>
//    public static bool IsDifferentFrom<T>(this T? newValue, T? currentValue) where T : struct, IEquatable<T>
//    {
//        // Both null = same
//        if (!newValue.HasValue && !currentValue.HasValue)
//            return false;

//        // One is null and the other isn't = different
//        if (newValue.HasValue != currentValue.HasValue)
//            return true;

//        // Both have values = compare them
//        return !newValue!.Value.Equals(currentValue!.Value);
//    }

//    /// <summary>
//    /// Checks if a non-nullable value is different from another.
//    /// Used for required fields.
//    /// </summary>
//    /// <typeparam name="T">The value type to compare.</typeparam>
//    /// <param name="newValue">The new value.</param>
//    /// <param name="currentValue">The current value.</param>
//    /// <returns>True if values are different.</returns>
//    public static bool IsDifferentFrom<T>(this T newValue, T currentValue) where T : struct, IEquatable<T>
//    {
//        return !newValue.Equals(currentValue);
//    }
//}