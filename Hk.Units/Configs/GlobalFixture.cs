using Hk.Core.Data.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using Hk.Core.Data.DbContextCore;
using Hk.Core.Data.DbContextCore.DbTypeContext;
using Hk.Core.Logs.Extensions;
using Hk.Core.Util.Extentions;

namespace Hk.Units.Configs
{
    /// <summary>
    /// 全局测试配置
    /// </summary>
    public class GlobalFixture
    {
        /// <summary>
        /// 测试初始化
        /// </summary>
        public GlobalFixture()
        {
            BuildServiceForSqlServer();
        }
        public IServiceProvider BuildServiceForSqlServer()
        {
            IServiceCollection services = new ServiceCollection();
            services = RegisterSqlServerContext(services);
            services.AddOptions();
            return services.AddUtil();
        }
        /// <summary>
        /// 注册上下文
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceCollection RegisterSqlServerContext(IServiceCollection services)
        {

            services.Configure<DbContextOption>(options =>
            {
                options.ConnectionString =
                    "Data Source=192.168.1.202;Initial Catalog=health_manager;port=3306;user id=root;password=mhealth365;charset=utf8";
                //options.ConnectionString =
                //    "Data Source=192.168.1.33;Initial Catalog=Hk.Core.Framework.DB;Persist Security Info=True;User ID=sa;Password=Huakang1949;MultipleActiveResultSets=True;";

                //options.ConnectionString =
                //    "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP) (HOST=121.41.14.20)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=testorcl)));Persist Security Info=True;User ID=HkGuide;Password=HkGuide;";
                //options.ModelAssemblyName = "Hk.Core.Entity";
            });

            //services.AddTransient<IDbContextCore, SqlServerDbContext>(); //注入上下文
                                                                         //services.AddScoped<IDbContextCore, OracleDbContext>(); //注入上下文
            services.AddScoped<IDbContextCore, MySqlDbContext>(); //注入上下文
            services.AddNLog();
            return services;
        }
    }
}