using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.InMemory;
using Hk.Core.Cache;
using Hk.Core.Cache.Extentions;
using Hk.Core.Data.DbContextCore;
using Hk.Core.Data.DbContextCore.DbTypeContext;
using Hk.Core.Data.Options;
using Hk.Core.Logs.Extensions;
using Hk.Core.Swagger;
using Hk.Core.Util;
using Hk.Core.Util.Extentions;
using Hk.IServices;
using Hk.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Hk.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region 配置DbContextOption
            //配置DbContextOption
            services.Configure<DbContextOption>(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString("MsSqlServer");
                options.ModelAssemblyName = "Hk.WebApis.Models";
            });
        
            #endregion
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddTransient(typeof(IDbContextCore), typeof(BaseDbContext));
            services.AddTransient<IDbContextCore, SqlServerDbContext>();
            services.AddTransientAssembly("Hk.IServices", "Hk.Services");
            services.AddSingleton(ApiInfo.Instantiate(Configuration));
            //services.AddSwagger();
            services.AddCustomSwagger(ApiInfo.Instance);
            services.AddNLog();
            //注册EasyCaching缓存
            services.AddEasyCachingForUtil(options=>options.UseInMemory());
            //支持获取Linux系统客户端的ip地址获取
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,IApiInfo apiInfo)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }          
            app.UseMvc()
               .UseCustomSwagger(apiInfo);
            //启用swagger
            //app.UseSwaggerEx();
        }
    }
}
