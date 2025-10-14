using System;
using System.Linq;
using TakalNew.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TakalNew.Data
{
    /// <summary>
    /// This class is used to run pending migrations and initialize DB with initial data if needed
    /// </summary>
    public static class PrepDb
    {
        public static async Task PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using( var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                
                if (context != null)
                {
                    await SeedData(context, isProd);
                }
            }
        }

        private static async Task SeedData(ApplicationDbContext context, bool isProd)
        {
            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not run migrations: {ex.Message}");
            }
        }
    }
}