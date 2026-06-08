using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Atlas.API.SwaggerConfiguration;

public static class SwaggerGenOptions
{
    public static IServiceCollection AddApplicationSwagger(this IServiceCollection services)
    {
        return services.AddSwaggerGen(c =>
        {
            c.SupportNonNullableReferenceTypes();
            c.SchemaFilter<SwaggerRequiredSchemaFilter>();
            c.SchemaFilter<EnumSchemaFilter>();
        });
    }
}

public class SwaggerRequiredSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null)
            return;

        if (schema is OpenApiSchema openApiSchema)
        {
            foreach (var schemaProps in schema.Properties)
            {
                openApiSchema.Required ??= new HashSet<string>();
                openApiSchema.Required?.Add(schemaProps.Key);
            }
        }
       
    }
}

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum && schema is OpenApiSchema openApiSchema)
        {
            var names = Enum.GetNames(context.Type);
            var res = new JsonArray();

            foreach (var i in names.Select(n => ToSnakeCase(n).ToUpper()))
            {
                res.Add(i);
            }

            openApiSchema.Extensions ??= new Dictionary<string, IOpenApiExtension>();
            openApiSchema.Extensions?.Add("x-enum-varnames", new JsonNodeExtension(res));
        }
    }

    private static string ToSnakeCase(string input)
    {
        return Regex.Replace(
            input,
            @"([a-z0-9])([A-Z])",
            "$1_$2"
        ).ToLower();
    }
}