namespace LoveYuri.Utils;

/// <summary>
/// 关于字符串的常用方法
/// </summary>
public static class StringUtils {
    /// <summary>
    /// 将字符串转换成float类型，如果转换失败则返回默认值
    /// </summary>
    /// <param name="str">待转换的字符串</param>
    /// <param name="defaultValue">失败后的值，默认0</param>
    public static float TryToFloat(this string str, float defaultValue = 0.0f) {
        return float.TryParse(str, out float value) ? value : defaultValue;
    }

    /// <summary>
    /// 将字符串转换成float类型，如果转换失败则抛出异常
    /// </summary>
    /// <param name="str">待转换的字符串</param>
    public static float ToFloat(this string str) {
        return float.Parse(str);
    }
    
    /// <summary>
    /// 将字符串转换成double类型，如果转换失败则返回默认值
    /// </summary>
    /// <param name="str">待转换的字符串</param>
    /// <param name="defaultValue">失败后的值，默认0</param>
    public static double TryToDouble(this string str, double defaultValue = 0.0) {
        return double.TryParse(str, out double value) ? value : defaultValue;
    }

    /// <summary>
    /// 将字符串转换成double类型，如果转换失败则抛出异常
    /// </summary>
    /// <param name="str">待转换的字符串</param>
    public static double ToDouble(this string str) {
        return double.Parse(str);
    }

    /// <summary>
    /// 将字符串转换成int类型，如果转换失败则抛出异常
    /// </summary>
    /// <param name="str">待转换的字符串</param>
    public static int ToInt(this string str) {
        return int.Parse(str);
    }

    /// <summary>
    /// 将字符串转换成int类型，如果转换失败则返回默认值
    /// </summary>
    /// <param name="str">待转换的字符串</param>
    /// <param name="defaultValue">失败后的值，默认0</param>
    public static int TryToInt(this string str, int defaultValue = 0) {
        return int.TryParse(str, out int value) ? value : defaultValue;
    }

    /// <summary>
    /// 将字符串格式化为枚举类型，如果失败则抛出异常
    /// </summary>
    public static T ToEnum<T>(this string v)
    {
        return (T)Enum.Parse(typeof(T), v);
    }

    /// <summary>
    /// 将字符串格式化为枚举类型，如果失败则抛出异常
    /// </summary>
    public static T TryToEnum<T>(this string v, T defaultValue = default) where T: struct, Enum
    {
        if (Enum.TryParse<T>(v, out var res)) {
            return res;
        }
        return defaultValue;
    }
}
