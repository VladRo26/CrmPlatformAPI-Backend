using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CrmPlatformAPI.Swagger
{

    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParam = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile))
                .FirstOrDefault();

            if (fileParam != null)
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                ["File"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                }
                            }
                        }
                    }
                }
                };
            }
        }
    }




}
