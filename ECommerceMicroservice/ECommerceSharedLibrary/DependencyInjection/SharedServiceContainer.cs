using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceSharedLibrary.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ECommerceSharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedService<TContext>(
            IServiceCollection service,
            IConfiguration config,
            string filename
        )
            where TContext : DbContext
        {
            service.AddDbContext<TContext>(option =>
                option.UseSqlServer(
                    config.GetConnectionString("ConnectionString"),
                    sqlserverOption => sqlserverOption.EnableRetryOnFailure()
                )
            );

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(
                    path: $"{filename}-.text",
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                    outputTemplate: "{Timestamp: dd-MM-yyyy HH:mm:ss. fff zzz} [{Level: u3}] {message: lj} {NewLine} {Exception}",
                    rollingInterval: RollingInterval.Day
                )
                .CreateLogger();

            JWTAuthenticationScheme.AddJWTAuthenticationScheme(service, config);

            return service;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalException>();
            app.UseMiddleware<ListenToOnlyApiGateway>();
            return app;
        }
    }
}
