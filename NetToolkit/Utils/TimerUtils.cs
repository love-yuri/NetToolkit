using Timer = System.Timers.Timer;

namespace LoveYuri.Utils;

/// <summary>
/// 定时器相关的工具
/// </summary>
public static class TimerUtils {
    /// <summary>
    /// n 毫秒后执行任务，只执行一次，执行结束自己会释放
    /// </summary>
    /// <param name="milliseconds">延迟时间(毫秒)</param>
    /// <param name="func">回调函数</param>
    /// <returns>可用于取消定时器的对象</returns>
    public static Timer Timeout(this int milliseconds, Action func) {
        var timer = new Timer(milliseconds);
        timer.AutoReset = false;
        timer.Elapsed += (_, _) => {
            try {
                func();
            } finally {
                timer.Stop();
                timer.Dispose();
            }
        };

        timer.Start();

        // 返回一个可以用于取消定时器的对象
        return timer;
    }

    /// <summary>
    /// 一直执行，直到n次
    /// </summary>
    /// <param name="milliseconds">每多少毫秒执行一次</param>
    /// <param name="func">待执行的函数</param>
    /// <param name="n">执行次数</param>
    public static Timer Interval(this int milliseconds, Action func, int? n = null) {
        var timer = new Timer(milliseconds);

        timer.AutoReset = true;
        var count = 0;
        timer.Elapsed += (_, _) => {
            try {
                func();
            } finally {
                count++;
                if (count >= n) {
                    timer.Stop();
                    timer.Dispose();
                }
            }
        };

        timer.Start();
        return timer;
    }
}
