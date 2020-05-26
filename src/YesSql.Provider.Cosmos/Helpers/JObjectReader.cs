using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Serialization;

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
            return (property.Value as JValue).Value != null ? (property.Value as JValue).Value.GetType() : GetValueType(property.Value as JValue);
        }

        private Type GetValueType(JValue jValue)
        {
            switch(jValue.Type)
            {
                case JTokenType.Integer:
                    {
                        return typeof(int);
                    }
                case JTokenType.Float:
                    {
                        return typeof(float);
                    }
                case JTokenType.Date:
                    {
                        return typeof(DateTime);
                    }
                case JTokenType.Boolean:
                    {
                        return typeof(bool);
                    }
                default:
                    {
                        return typeof(string);
                    }
            }
        }

        public string GetPropertyName(int ordinal)
        {
            var property = Data.Properties().Skip(ordinal).FirstOrDefault();
            return property.Name;
        }

        public int GetPropertyIndex(string name)
        {
            var properties = Data.Properties().ToArray();
            int index = 0;
            foreach(var property in properties)
            {
                if(property.Name == name)
                {
                    break;
                }
                index++;
            }
            return index;
        }
    }
}
