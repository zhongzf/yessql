using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace YesSql.Provider.Cosmos.Helpers
{
    public class JObjectReader
    {
        public JObject Data { get; set; }

        public JObjectReader(JObject data)
        {
            Data = data;
        }

        public object GetValue(int ordinal)
        {
            var property = Data.Properties().Skip(ordinal).FirstOrDefault();
            return (property.Value as JValue).Value;
        }
    }
}
