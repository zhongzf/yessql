using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YesSql.Provider.Cosmos.Helpers
{
    public class CosmosExecutor
    {
        public const string PartitionKey = "__partition__";
        public const string PartitionKeyPath = "/" + PartitionKey;
        public const string DefaultPartitionKey = "0";
        public const string KeyName = "id";

        public CosmosClient Client { get; set; }
        public string DatabaseId { get; set; }

        public Database Database
        {
            get
            {
                return Client.CreateDatabaseIfNotExistsAsync(DatabaseId).GetAwaiter().GetResult();
            }
        }

        public FeedIterator Query(string tableName, string commandText, DbParameterCollection parameters, out string queryText)
        {
            queryText = new CommandConverter(commandText).Convert();
            var queryDefinition = new QueryDefinition(queryText);
            if(parameters != null && parameters.Count > 0)
            {
                foreach(DbParameter parameter in parameters)
                {
                    var parameterName = parameter.ParameterName.StartsWith("@") ? parameter.ParameterName : "@" + parameter.ParameterName;
                    var parameterValue = parameter.Value;
                    queryDefinition = queryDefinition.WithParameter(parameterName, parameterValue);
                }
            }
            var containerId = tableName;
            Container container = Database.CreateContainerIfNotExistsAsync(containerId, PartitionKeyPath).GetAwaiter().GetResult();
            return container.GetItemQueryStreamIterator(queryDefinition);
        }

        public async Task<object> CreateAsync(string containerId, object data)
        {
            Container container = await Database.CreateContainerIfNotExistsAsync(containerId, PartitionKeyPath);
            EnsureDefaultId(data);
            return await container.CreateItemStreamAsync(ConvertToStream(data), new PartitionKey(DefaultPartitionKey));
        }

        public Stream ConvertToStream(object data)
        {
            var content = JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(content);
            return new MemoryStream(bytes);
        }

        private static void EnsureDefaultId(object data)
        {
            var dictionary = data as IDictionary<string, object>;
            if (dictionary != null)
            {
                if (!dictionary.ContainsKey(KeyName))
                {
                    dictionary.Add(KeyName, Guid.NewGuid().ToString());
                }
                if (!dictionary.ContainsKey(PartitionKey))
                {
                    dictionary.Add(PartitionKey, DefaultPartitionKey);
                }
            }
        }
    }
}
