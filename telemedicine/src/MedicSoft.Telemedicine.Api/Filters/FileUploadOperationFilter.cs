using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace MedicSoft.Telemedicine.Api.Filters;

/// <summary>
/// Swagger operation filter that handles file upload endpoints with [FromForm] and IFormFile parameters.
/// This filter ensures proper schema generation for multipart/form-data requests.
/// </summary>
public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if the endpoint consumes multipart/form-data
        var consumesAttribute = context.MethodInfo.GetCustomAttributes(true)
            .OfType<ConsumesAttribute>()
            .FirstOrDefault();
            
        if (consumesAttribute == null || 
            !consumesAttribute.ContentTypes.Any(ct => ct.Contains("multipart/form-data")))
            return;

        var parameters = context.ApiDescription.ParameterDescriptions;
        
        // Check if any parameter is IFormFile
        var hasFormFile = parameters.Any(p => 
            p.Type == typeof(IFormFile) || 
            p.Type == typeof(IFormFileCollection) ||
            p.Type == typeof(IFormFile[]));
        
        if (!hasFormFile)
            return;

        // Clear existing parameters as we'll define them in the request body
        operation.Parameters?.Clear();

        // Create the request body schema
        var schema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>(),
            Required = new HashSet<string>()
        };

        foreach (var parameter in parameters.Where(p => p.Source.Id == "Form" || p.Source.Id == "FormFile"))
        {
            if (parameter.Type == typeof(IFormFile))
            {
                // Add file upload parameter
                schema.Properties.Add(parameter.Name, new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary",
                    Description = parameter.ModelMetadata?.Description
                });

                // documentPhoto is required based on controller logic
                if (parameter.Name == "documentPhoto")
                {
                    schema.Required.Add(parameter.Name);
                }
            }
            else if (!IsSimpleType(parameter.Type))
            {
                // Complex type - expand its properties
                var typeProperties = parameter.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                
                foreach (var prop in typeProperties)
                {
                    var propName = prop.Name.Length > 1 
                        ? char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1)
                        : prop.Name.ToLowerInvariant();
                    var propSchema = CreateSchemaForType(prop.PropertyType);
                    schema.Properties.Add(propName, propSchema);
                    
                    // Check if property is required (non-nullable value type or has Required attribute)
                    if (prop.PropertyType.IsValueType && Nullable.GetUnderlyingType(prop.PropertyType) == null)
                    {
                        schema.Required.Add(propName);
                    }
                }
            }
            else
            {
                // Simple type parameter
                var propSchema = CreateSchemaForType(parameter.Type);
                schema.Properties.Add(parameter.Name, propSchema);
            }
        }

        // Add header parameters back
        foreach (var parameter in context.ApiDescription.ParameterDescriptions.Where(p => p.Source.Id == "Header"))
        {
            operation.Parameters ??= new List<OpenApiParameter>();
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = parameter.Name,
                In = ParameterLocation.Header,
                Required = parameter.IsRequired,
                Schema = CreateSchemaForType(parameter.Type)
            });
        }

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = schema
                }
            }
        };
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(Guid)
            || Nullable.GetUnderlyingType(type) != null;
    }

    private static OpenApiSchema CreateSchemaForType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        
        if (underlyingType == typeof(string))
            return new OpenApiSchema { Type = "string" };
        if (underlyingType == typeof(int) || underlyingType == typeof(long))
            return new OpenApiSchema { Type = "integer", Format = underlyingType == typeof(long) ? "int64" : "int32" };
        if (underlyingType == typeof(bool))
            return new OpenApiSchema { Type = "boolean" };
        if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateTimeOffset))
            return new OpenApiSchema { Type = "string", Format = "date-time" };
        if (underlyingType == typeof(Guid))
            return new OpenApiSchema { Type = "string", Format = "uuid" };
        if (underlyingType == typeof(decimal) || underlyingType == typeof(double) || underlyingType == typeof(float))
            return new OpenApiSchema { Type = "number", Format = "double" };
            
        return new OpenApiSchema { Type = "string" };
    }
}
