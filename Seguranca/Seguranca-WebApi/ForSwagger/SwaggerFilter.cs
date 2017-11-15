using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Seguranca_WebApi.ForSwagger
{
    public class SwaggerFilter : IOperationFilter
{
    public void Apply(Operation operation, OperationFilterContext context)
    {
        var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
        var isAuthorized = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ApiDescription.ActionDescriptor).MethodInfo.CustomAttributes.Any(att => att.AttributeType == typeof(AuthorizeAttribute));

        if (isAuthorized)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "Authorization",
                In = "header",
                Description = "access token",
                Required = true,
                Type = "string"
            });
        }
    }
}
}
