using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoveYuri.Core.Service;

/// <summary>
/// 全局di服务
/// </summary>
public class DiService: IAsyncDisposable {
    private static IHost? _host;

    /// <summary>
    /// 全局服务提供
    /// </summary>
    private static IServiceProvider? ServiceProvider => _host?.Services;

    /// <summary>
    /// 检查DI服务是否已初始化
    /// </summary>
    public static bool IsInitialized => ServiceProvider != null;

    /// <summary>
    /// 获取某个类型的依赖服务，如果不存在则报错
    /// </summary>
    public static T GetRequiredService<T>() where T : notnull
    {
        if (ServiceProvider == null) {
            throw new InvalidOperationException("DI服务尚未初始化");
        }

        return ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// 获取某个类型的依赖服务，如果不存在则报错
    /// </summary>
    public static object GetRequiredService(Type type) {
        if (ServiceProvider == null) {
            throw new InvalidOperationException("DI服务尚未初始化");
        }

        return ServiceProvider.GetRequiredService(type);
    }

    /// <summary>
    /// 获取某个类型的依赖服务，如果不存在则返回null
    /// </summary>
    public static T? GetService<T>() where T : class {
        return ServiceProvider?.GetService<T>();
    }

    private DiService()
    {
        if (_host == null) {
            throw new InvalidOperationException("DI服务尚未初始化");
        }

        _host.Start();
    }

    /// <summary>
    /// 注册DiService
    /// </summary>
    /// <param name="register">服务注册函数</param>
    public static DiService RegisterDiService(Action<IServiceCollection> register) {
        if (_host != null) {
            throw new InvalidOperationException("DI服务已经初始化");
        }

        _host = Host
            .CreateDefaultBuilder()
            .ConfigureLogging(logging => logging.ClearProviders()) // 移除所有日志提供程序
            .ConfigureServices((_, service) => {
                // 注册服务
                register.Invoke(service);
            }).Build();

        return new DiService();
    }


    private static void ReleaseUnmanagedResources()
    {
        if (_host != null) {
            _host.Dispose();
            _host = null;
        }
    }

    public ValueTask DisposeAsync()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    ~DiService()
    {
        ReleaseUnmanagedResources();
    }
}
