using Hk.Core.Logs;
using Xunit;
using Xunit.Abstractions;

namespace Hk.Units
{
    [Collection("GlobalConfig")]
    public class logTest
    {
        private readonly ITestOutputHelper _output;

        public logTest(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void OutLog()
        {
            Log.GetLog("测试日志").Info("测试中...");
        }
    }
}