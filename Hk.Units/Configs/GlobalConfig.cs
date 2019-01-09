using Xunit;

namespace Hk.Units.Configs
{
    /// <summary>
    /// 全局测试配置
    /// </summary>
    [CollectionDefinition("GlobalConfig")]
    public class GlobalConfig : ICollectionFixture<GlobalFixture>
    {

    }
}