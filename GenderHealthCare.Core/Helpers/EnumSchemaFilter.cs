using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GenderHealthCare.Core.Helpers
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!context.Type.IsEnum) return;

            var enumDescriptions = Enum.GetNames(context.Type)
                .Select(name =>
                {
                    var value = Convert.ToInt32(Enum.Parse(context.Type, name));
                    return $"{value} = {name}";
                });

            schema.Description += $"\nPossible values: {string.Join(", ", enumDescriptions)}";
        }
    }
}