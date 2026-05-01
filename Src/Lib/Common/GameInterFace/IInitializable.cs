namespace GameInterFace
{
    /// <summary>
    /// LoadingManager 初始化时会尝试调用所有继承该接口的Single类的 Init 方法。
    /// </summary>
    public interface IInitializable
    {
        public void Init();
    }
}
