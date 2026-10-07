using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Extensions
{
    public  static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommon(this IServiceCollection services) 
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
            });
            return services;
        }
    }
}
