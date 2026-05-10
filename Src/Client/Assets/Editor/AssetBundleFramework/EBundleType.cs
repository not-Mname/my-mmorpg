namespace AssetBundleFramework
{
    /// <summary>
    /// AssetBundle打包粒度类型枚举
    /// 定义资源被打包的精细程度，直接影响Bundle的数量和大小
    /// File粒度最小但数量最多，All粒度最大但数量最少
    /// 需要根据项目需求和资源特点选择合适的粒度策略
    /// </summary>
    public enum EBundleType
    {
        /// <summary>
        /// 文件级打包粒度（最小颗粒度）
        /// 每个文件单独打包成一个AssetBundle
        /// </summary>
        File,
        
        /// <summary>
        /// 目录级打包粒度（中等颗粒度）
        /// 同一目录下的所有资源打包成一个AssetBundle
        /// </summary>
        Directory,
        
        /// <summary>
        /// 全部打包粒度（最大颗粒度）
        /// 将指定路径下的所有资源打包成一个AssetBundle
        /// </summary>
        All
    }
}