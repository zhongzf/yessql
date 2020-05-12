using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using YesSql.Provider.Cosmos.Client;

namespace YesSql.Provider.Cosmos
{
    public static class CosmosDbProviderOptionsExtensions
    {
        public static IConfiguration RegisterCosmos(this IConfiguration configuration)
        {
            SqlDialectFactory.SqlDialects["cosmosconnection"] = new CosmosDialect();
            CommandInterpreterFactory.CommandInterpreters["cosmosconnection"] = d => new CosmosCommandInterpreter(d);

            return configuration;
        }

        public static IConfiguration UseCosmos(
            this IConfiguration configuration,
            string connectionString)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (String.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(nameof(connectionString));
            }

            RegisterCosmos(configuration);

            configuration.ConnectionFactory = new DbConnectionFactory<CosmosConnection>(connectionString);

            return configuration;
        }
    }
}
