using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace Hk.Core.Util.Extentions
{
    public static partial class Extensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {

            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info
                    {
                        Version = "v1",
                        Title = "Hk.WebApis"
                    });

                    //c.SwaggerDoc("v2", new Info
                    //{
                    //    Version = "v2",
                    //    Title = "Hk.WebApis",
                    //    Description = "Sample Web API",
                    //    Contact = new Contact() { Name = "Talking Dotnet", Email = "275979650@qq.com", Url = "www.chendong.com" }
                    //});
                    // Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                    c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                    {
                        Type = "oauth2",
                        Flow = "implicit",
                        AuthorizationUrl = "http://petstore.swagger.io/oauth/dialog",
                        Scopes = new Dictionary<string, string>
                        {
                            { "readAccess", "Access read operations" },
                            { "writeAccess", "Access write operations" }
                        }
                    });


                    var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                    var xmlPath = Path.Combine(basePath, "Hk.WebApi.xml");
                    c.IncludeXmlComments(xmlPath);
                }
            );
            return services;
        }

        public static void UseSwaggerEx(this IApplicationBuilder app)
        {
            //启用swagger
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hk.WebApis v1");
                //c.SwaggerEndpoint("/swagger/v2/swagger.json", "Hk.WebApis v2");
                c.OAuthClientId("test-id");
                c.OAuthClientSecret("test-secret");
                c.OAuthRealm("test-realm");
                c.OAuthAppName("test-app");
                c.OAuthScopeSeparator(" ");
                c.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { { "foo", "bar" } });
                c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });
        }
    }
}