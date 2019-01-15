using System;
using EasyCaching.Core;
using EasyCaching.Core.Internal;
using EasyCaching.InMemory;
using EasyCaching.Redis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hk.Core.Cache.Extentions
{
    public static partial class Extensions
    {        /// <summary>
        /// 注册EasyCaching缓存操作
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configAction">配置操作</param>
        public static void AddEasyCachingForUtil(this IServiceCollection services, Action<EasyCachingOptions> configAction)
        {
            services.TryAddScoped<ICache, CacheManager>();
            services.AddEasyCaching(configAction);
        }
    }
}