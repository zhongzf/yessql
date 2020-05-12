using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YesSql.Provider.Cosmos.Helpers
{
    public class CosmosExecutor
    {
        public CosmosClient Client { get; set; }
        public string DatabaseId { get; set; }

        public Database Database
        {
            get
            {
                return Client.CreateDatabaseIfNotExistsAsync(DatabaseId).GetAwaiter().GetResult();
            }
        }

        public FeedIterator Query(string commandText)
        {
            var queryText = commandText.TrimEnd(';');
            // TODO: SELECT Document.* FROM Document
            return Database.GetContainerQueryStreamIterator(queryText);
        }

        public async Task<object> Create(string containerId, object data)
        {
            Container container = await Database.CreateContainerIfNotExistsAsync(containerId, "Id");
            return await container.CreateItemAsync<object>(data);
        }
    }
}
