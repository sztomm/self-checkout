using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using SelfCheckout.API.Helpers;

namespace SelfCheckout.API.Extensions
{
    public static class SwaggerExtension
    {
        public static void AddApiDoc(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(@".\SelfCheckout.API.xml");
            });

            services.ConfigureOptions<ConfigureSwaggerOptions>();
        }

        public static IApplicationBuilder UseApiDocs(this IApplicationBuilder app)
        {
            var provider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();

            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"../swagger/{description.GroupName}/swagger.json",
                                            $"Self Checkout API {description.GroupName.ToUpperInvariant()}");
                }
            });
            return app;
        }
    }
}
