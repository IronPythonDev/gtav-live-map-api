﻿using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Contexts
{
    public class DbContext
    {
        public DbContext(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("PostgreSQLDocker");
            Configuration = configuration;

            EnsureCreated();
        }

        IConfiguration Configuration { get; }

        private string connectionString { get; }

        public async void CreateDatabase(string name)
        {
            var connection = GetConnection();

            await connection.ExecuteAsync($"CREATE DATABASE \"{name}\"");
        }

        public async Task<bool> DBExistsAsync(string dbName)
        {
            var connection = GetConnection(false);

            var result = await connection.QueryAsync($"SELECT * FROM pg_database WHERE datname = '{dbName}';");

            return result.Count() > 0;
        }

        public async void EnsureCreated()
        {
            var dbName = Configuration["PostgreConfig:DataBaseName"];

            var isExistsDB = await DBExistsAsync(dbName);

            if (!isExistsDB)
                CreateDatabase(dbName);
        }

        public async Task<IEnumerable<TReturn>> Query<TReturn>(string sql , object param)
        {
            var connection = GetConnection();

            return await connection.QueryAsync<TReturn>(sql, param);
        }

        public async void Execute(string sql, object param)
        {
            var connection = GetConnection();

            await connection.ExecuteAsync(sql, param);
        }

        public NpgsqlConnection GetConnection(bool isUseDBNameFromConfig = true) =>
            new NpgsqlConnection(connectionString + (isUseDBNameFromConfig ? $"Database={Configuration["PostgreConfig:DataBaseName"]}" : ""));
    }
}
