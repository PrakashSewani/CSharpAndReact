using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Domain;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host =CreateHostBuilder(args).Build();

            using var scope=host.Services.CreateScope();

            var services=scope.ServiceProvider;

            try{
                var userManager=services.GetRequiredService<UserManager<AppUser>>();
                var context=services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync();
                await Seed.SeedData(context,userManager);

            }catch (Exception ex){
                var logger=services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error has occurred during Migration");
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
