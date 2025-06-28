using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HiveCardAPI.Helpers
{
    public class FileUploadOperation : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            Debug.WriteLine($"⚡️ FileUploadOperation running for {context.MethodInfo.Name}");

            var fileParams = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile));

            if (fileParams.Any())
            {
                // ✅ Clear default parameters — REQUIRED
                operation.Parameters.Clear();

                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = {
                        ["multipart/form-data"] = new OpenApiMediaType {
                            Schema = new OpenApiSchema {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema> {
                                    ["file"] = new OpenApiSchema {
                                        Type = "string",
                                        Format = "binary"
                                    },
                                    ["userId"] = new OpenApiSchema {
                                        Type = "integer",
                                        Format = "int32"
                                    }
                                },
                                Required = new HashSet<string> { "file", "userId" }
                            }
                        }
                    }
                };
            }
        }
    }
}