

using System;
using EasyCaching.InMemory;
using Hk.Core.Cache;
using Hk.Core.Cache.Extentions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Hk.Units
{
    public class cacheTest
    {
        private readonly ICache _cache;
        private readonly ITestOutputHelper _output;
        public cacheTest(ITestOutputHelper output)
        {
            var services = new ServiceCollection();
            services.AddEasyCachingForUtil(options => options.UseInMemory());
            var serviceProvider = services.BuildServiceProvider();
            _cache = serviceProvider.GetService<ICache>();
            _output = output;
        }
        [Fact]
        public void TestGet()
        {
            var res1= _cache.Get("demo", () => "456",TimeSpan.FromMinutes(1));
            _output.WriteLine("当前demo缓存：" + res1);
            Assert.True(_cache.TryAdd("demo1",res1, TimeSpan.FromMinutes(1)));
            //var res = _cache.Get<string>("demo1");
            //_output.WriteLine("当前demo1缓存：" + res);
        }
        [Fact]
        public void TestGet1()
        {
            var res = _cache.Get<string>("demo1");
            _output.WriteLine("当前demo1缓存：" + res);
        }
    }

}