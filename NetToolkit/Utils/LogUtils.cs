using System.Runtime.InteropServices;

namespace LoveYuri.Utils;

public enum WriteMode : byte {
    /// 仅写入控制台
    WriteInConsole = 0x01,

    /// 仅写入文件
    WriteInFile = 0x02,

    /// 同时写入控制台和文件
    WriteInConsoleAndFile = WriteInConsole | WriteInFile
}

/// <summary>
/// 简单日志类
/// </summary>
public static partial class Log {
    /// <summary>
    /// 打印日志 info级别
    /// </summary>
    /// <param name="msg"></param>
    [LibraryImport("LoveYuri.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial void Info(string msg);

    /// <summary>
    /// 输出到标准错误流
    /// </summary>
    /// <param name="msg"></param>
    [LibraryImport("LoveYuri.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial void Error(string msg);

    [LibraryImport("LoveYuri.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial void Warn(string msg);

    [LibraryImport("LoveYuri.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static partial void SetLogFilePath(string msg);

    [LibraryImport("LoveYuri.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static partial void SetWriteMode(byte mode);

    /// <summary>
    /// 将日志添加到文件
    /// </summary>
    /// <param name="file">文件路径</param>
    /// <param name="appendMode">追加模式，默认同时写入控制台和文件</param>
    public static void AppendLogToFile(string file, WriteMode appendMode = WriteMode.WriteInConsoleAndFile)
    {
        SetWriteMode((byte)appendMode);
        SetLogFilePath(file);
    }

    /// <summary>
    /// 设置日志写入模式
    /// </summary>
    /// <param name="mode"></param>
    public static void SetLogWriteMode(WriteMode mode)
    {
        SetWriteMode((byte)mode);
    }

    /// <summary>
    /// 格式化消息日志
    /// </summary>
    /// <param name="level">消息等级 - 可以自定义</param>
    /// <param name="msg">消息</param>
    /// <returns></returns>
    public static string FormatMsg(object level, string msg) {
        var time = DateTime.Now.ToString("HH:mm:ss.fff");
        string levelStr = level.ToString()!.PadLeft(5);
        return $"[{time} {levelStr}] {msg}";
    }

    /// <summary>
    /// Debug模式，只在debug下打印，级别等同于info
    /// </summary>
    /// <param name="msg"></param>
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Debug(string msg) => Info(msg);
}
