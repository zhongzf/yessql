using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YesSql.Provider.Cosmos.Helpers
{
    public class FeedIteratorReader
    {
        public FeedIterator FeedIterator { get; set; }

        public string CommandText { get; set; }
        public string QueryText { get; set; }

        public async Task<object> ReadNext()
        {
            using (ResponseMessage response = await FeedIterator.ReadNextAsync())
            {
                return new ResponseReader { Response = response }.Read();
            }
        }
    }
}
