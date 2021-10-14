using Dapper;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core
{
    public class Program
    {
        public static IHost WebHost { get; private set; }

        public static void Main(string[] args)
        {
            WebHost = CreateHostBuilder(args).Build();

            MigrateDB(WebHost.Services);

            WebHost.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void MigrateDB(IServiceProvider provider)
        {
            var scope = provider.CreateScope();

            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();

            scope.Dispose();
        }
    }
}
