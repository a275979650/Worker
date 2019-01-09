﻿using System;
using Hk.Core.Util.Enum;
using Hk.Core.Util.Extentions;
using Hk.Core.Util.Helper;
using Microsoft.Extensions.Configuration;

namespace Hk.Core.Util.Cache
{
    /// <summary>
    /// 缓存帮助类
    /// </summary>
    public class CacheHelper
    {
        /// <summary>
        /// 静态构造函数，初始化缓存类型
        /// </summary>
        static CacheHelper()
        {
            SystemCache = new SystemCache();
            string cacheType = ConfigHelper.GetSection("SystemConfig", "CacheType");
            string redisConfig = ConfigHelper.GetSection("SystemConfig", "RedisConfig");
            switch (cacheType)
            {
                case "SystemCache":
                    Cache = SystemCache;
                    break;
                case "RedisCache":
                    Cache = RedisCache;
                    if (!string.IsNullOrEmpty(redisConfig))
                    {
                        RedisCache = new RedisCache(redisConfig);
                    }
                    break;
                default: throw new Exception("请指定缓存类型！");
            }
        }

        /// <summary>
        /// 默认缓存
        /// </summary>
        public static ICache Cache { get; }

        /// <summary>
        /// 系统缓存
        /// </summary>
        public static ICache SystemCache { get; }

        /// <summary>
        /// Redis缓存
        /// </summary>
        public static ICache RedisCache { get; }
    }
}
