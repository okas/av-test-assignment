using System.Reflection;
using Backend.WebApi.Utilities;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backend.WebApi.Swagger;

public static class SwaggerGenExtensionMethods
{
    /// <summary>
    ///     Inject human-friendly descriptions for Operations, Parameters and Schemas based on XML Comment files
    ///     This overload uses code's .NET XML comments to get additional API description info.
    ///     You need just need to turn on XML documentation generation form projects properties.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="includeControllerXmlComments">
    ///     Flag to indicate if controller XML comments (i.e. summary) should be used to
    ///     assign Tag descriptions. Don't set this flag if you're customizing the default
    ///     tag for operations via TagActionsBy.
    /// </param>
    public static void IncludeXmlCommentsOfCurrentProject(this SwaggerGenOptions options,
        bool includeControllerXmlComments = true)
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments);
    }

    /// <summary>
    /// Uses RouteValues to generate OAS OperationId using pattern, in camelCase: "{controller}{action}".
    /// </summary>
    /// <param name="options"></param>
    public static void CustomOperationIdsControllerAndActionCamelCase(this SwaggerGenOptions options)
    {
        options.CustomOperationIds(apiDesc =>
        {
            var controller = apiDesc.ActionDescriptor.RouteValues["controller"];
            var action = apiDesc.ActionDescriptor.RouteValues["action"];
            return StringUtilities.ToCamelCase($"{controller}{action}");
        });
    }
}
