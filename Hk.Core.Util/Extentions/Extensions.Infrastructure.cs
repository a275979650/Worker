using System;
using System.Text;
using Hk.Core.Util.Contexts;
using Hk.Core.Util.Dependency;
using Microsoft.Extensions.DependencyInjection;

namespace Hk.Core.Util.Extentions
{
    /// <summary>
    /// 系统扩展 - 基础设施
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 注册Util基础设施服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configs">依赖配置</param>
        public static IServiceProvider AddUtil(this IServiceCollection services, params IConfig[] configs)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Bootstrapper.Run(services, new WebContext(), configs);
        }
    }
}