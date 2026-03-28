using System;
using System.Runtime.InteropServices;

/// <summary>
/// 高精度时间工具类，提供游戏时间相关功能
/// </summary>
class Time
{
    // 导入Windows高性能计数器API
    [DllImport("kernel32.dll")]
    static extern bool QueryPerformanceCounter([In, Out] ref long lpPerformanceCount);
    [DllImport("kernel32.dll")]
    static extern bool QueryPerformanceFrequency([In, Out] ref long lpFrequency);

    /// <summary>
    /// 静态构造函数，初始化启动时间戳
    /// </summary>
    static Time()
    {
        startupTicks = ticks; // 记录程序启动时的时间戳
    }

    private static long _frameCount = 0;

    /// <summary>
    /// 获取已运行的帧数（只读）
    /// </summary>
    public static long frameCount { get { return _frameCount; } }

    static long startupTicks = 0; // 程序启动时的时间戳
    static long freq = 0;         // 性能计数器频率

    /// <summary>
    /// 获取当前高精度时间计数（以100纳秒为单位）
    /// 1. 优先使用高性能计数器
    /// 2. 如果不可用则回退到系统时钟
    /// </summary>
    static public long ticks
    {
        get
        {
            long f = freq;

            // 首次调用时初始化频率
            if (f == 0)
            {
                if (QueryPerformanceFrequency(ref f))
                {
                    freq = f; // 记录成功获取的频率
                }
                else
                {
                    freq = -1; // 标记高性能计数器不可用
                }
            }

            // 高性能计数器不可用时回退到系统时钟
            if (f == -1)
            {
                return Environment.TickCount * 10000; // 转换为100纳秒单位
            }

            // 使用高性能计数器获取当前时间
            long c = 0;
            QueryPerformanceCounter(ref c);
            return (long)(((double)c) * 1000 * 10000 / ((double)f)); // 转换为100纳秒单位
        }
    }

    private static long lastTick = 0;     // 上一帧的时间戳
    private static float _deltaTime = 0;  // 上一帧耗时（秒）

    /// <summary>
    /// 获取上一帧的耗时（秒，只读）
    /// </summary>
    public static float deltaTime
    {
        get
        {
            return _deltaTime;
        }
    }


    private static float _time = 0; // 游戏运行时间（秒）

    /// <summary>
    /// 获取当前帧开始时的游戏时间（秒，只读）
    /// 从游戏开始计时
    /// </summary> 
    public static float time
    {
        get
        {
            return _time;
        }
    }


    /// <summary>
    /// 获取从程序启动开始的真实时间（秒，只读）
    /// </summary>
    /// <returns>从启动开始的秒数</returns>
    public static float realtimeSinceStartup
    {
        get
        {
            long _ticks = ticks;
            return (_ticks - startupTicks) / 10000000f; // 将100纳秒转换为秒
        }
    }

    /// <summary>
    /// 更新时间统计（每帧调用）
    /// 1. 更新帧计数器
    /// 2. 计算帧间隔时间
    /// 3. 更新游戏运行时间
    /// </summary>
    public static void Tick()
    {
        long _ticks = ticks; // 获取当前时间戳

        // 更新帧计数器（处理溢出）
        _frameCount++;
        if (_frameCount == long.MaxValue)
            _frameCount = 0;

        // 初始化上一帧时间（首帧）
        if (lastTick == 0) lastTick = _ticks;

        // 计算帧间隔时间（秒）
        _deltaTime = (_ticks - lastTick) / 10000000f;

        // 更新游戏运行时间（秒）
        _time = (_ticks - startupTicks) / 10000000f;

        // 记录当前帧时间
        lastTick = _ticks;
    }
}