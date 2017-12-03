using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace OrderService.Auth
{
    public class AddAuthTokenHeader : IOperationFilter
    {
        public void Apply(Operation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            operation.Parameters.Add(new HeaderParameter()
            {
                Name = "AuthToken",
                In = "header",
                Type = "string",
                Required = false
            });
        }
    }

    internal class HeaderParameter : NonBodyParameter{}
}
