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

        public object GetValue(string name)
        {
            return (Data.GetValue(name) as JValue).Value;
        }

        public JProperty[] GetProperties()
        {
            return Data.Properties().ToArray();
        }

        public Type GetPropertyType(int ordinal)
        {
            var property = Data.Properties().Skip(ordinal).FirstOrDefault();
            return (property.Value as JValue).Value.GetType();
        }

        public string GetPropertyName(int ordinal)
        {
            var property = Data.Properties().Skip(ordinal).FirstOrDefault();
            return property.Name;
        }

        public int GetPropertyIndex(string name)
        {
            // TODO:
            return 0;
        }
    }
}
