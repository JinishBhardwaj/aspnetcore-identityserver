using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Weather.API.Core.Swagger
{
    /// <summary>
    /// This operation filter tells swagger document which endpoints
    /// require an access token to work and these endpoints return
    /// 401 and 403 responses.
    /// </summary>
    public class AuthorizeOperationFilter : IOperationFilter
    {
        #region Implementation of IOperationFilter

        /// <inheritdoc />
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Since all the operations in our api are protected, we need not
            // check separately if the operation has Authorize attribute
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2"}
                        }
                    ] = new[] {"weatheapi"}
                }
            };
        }

        #endregion
    }
}
