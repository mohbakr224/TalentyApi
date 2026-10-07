using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
namespace Talenty.OpenApi
{
    public static class OpenApiExtensions
    {
        public static IServiceCollection AddOpenApi(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Talenty API", Version = "v1" });

                // Add JWT bearer auth support in Swagger UI so frontend can authorize requests
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token in the text input below."
                });

                // Only show the lock icon for operations that require authorization.
                // This OperationFilter will add the security requirement per-operation when [Authorize] is present.
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            return services;
        }

        public static WebApplication MapOpenApi(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Talenty API v1");
                c.RoutePrefix = "swagger";
            });
            return app;
        }
    }

    // OperationFilter that adds the Bearer security requirement only to endpoints that have [Authorize]
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAllowAnonymous = context.MethodInfo?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() == true;
            if (hasAllowAnonymous)
                return;

            var hasAuthorize = (context.MethodInfo?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true)
                || (context.MethodInfo?.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true);

            if (!hasAuthorize)
                return;

            operation.Security ??= new List<OpenApiSecurityRequirement>();

            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            };

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [scheme] = new List<string>()
            });
        }
    }
}
