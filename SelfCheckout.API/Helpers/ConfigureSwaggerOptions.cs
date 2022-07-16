using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SelfCheckout.API.Helpers
{
    public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider Provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            Provider = provider;
        }

        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in Provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo()
                    {
                        Title = "Self Checkout API",
                        Version = description.ApiVersion.ToString().ToUpperInvariant(),
                        Description = description.IsDeprecated ? " This API version has been deprecated" : ""
                    });
            }
        }
    }
}
