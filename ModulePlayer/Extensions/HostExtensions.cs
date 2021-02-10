using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;
using Polly;

namespace ModulePlayer.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetService<TContext>();

                try
                {

                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(new TimeSpan[]
                        {
                            TimeSpan.FromSeconds(3),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(8),
                        });

                    retry.Execute(() =>
                    {
                        // If the sql server container is not created on run docker compose this
                        // migration can't fail for network related exception. The retry options for DbContext only 
                        // apply to transient exceptions.
                        context.Database.Migrate();

                        // Apply migrations
                        seeder(context, services);
                    });

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return host;
        }
    }
}
