using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YesSql.Provider.Cosmos.Helpers
{
    public class ResponseReader
    {
        public ResponseMessage Response { get; set; }

        public object Read()
        {
            var response = Response;
            if (response.Diagnostics != null)
            {
                Console.WriteLine($"ItemStreamFeed Diagnostics: {response.Diagnostics.ToString()}");
            }

            response.EnsureSuccessStatusCode();
            using (StreamReader sr = new StreamReader(response.Content))
            using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                var value = jsonSerializer.Deserialize(jsonTextReader);
                return value;
            }
        }
    }
}
