// EventMode.cs
namespace Utilities
{
    /// <summary>
    /// 事件模式
    /// </summary>
    public enum EventMode
    {
        /// <summary>
        /// 单播：一个事件名只能有一个监听器，后订阅覆盖前订阅
        /// </summary>
        Unicast,

        /// <summary>
        /// 多播：一个事件名可以有多个监听器，触发时依次调用
        /// </summary>
        Multicast
    }
}
