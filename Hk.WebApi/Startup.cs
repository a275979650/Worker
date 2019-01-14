using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hk.Core.Data.DbContextCore;
using Hk.Core.Data.DbContextCore.DbTypeContext;
using Hk.Core.Data.Options;
using Hk.Core.Util.Extentions;
using Hk.IServices;
using Hk.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddTransient(typeof(IDbContextCore), typeof(BaseDbContext));
            services.AddTransient<IDbContextCore, SqlServerDbContext>();
            services.AddTransientAssembly("Hk.IServices", "Hk.Services");
            services.AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }          
            app.UseMvc();
            //启用swagger
            app.UseSwaggerEx();
        }
    }
}
