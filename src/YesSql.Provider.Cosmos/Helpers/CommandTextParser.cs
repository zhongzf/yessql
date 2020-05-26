using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace YesSql.Provider.Cosmos.Helpers
{
    public class CommandTextParser
    {
        public const string ParameterIdentifying = "@";

        public string ExtractTableName(string commandText)
        {
            var tableName = string.Empty;
            var regexTableName = new Regex("SELECT .* FROM ([\\w\\d]*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (regexTableName.IsMatch(commandText))
            {
                var groups = regexTableName.Match(commandText).Groups;
                tableName = groups[1].Value;
            }
            return tableName;
        }

        private Dictionary<string, object> ExtractColumnsValue(string columnsString, DbParameterCollection parameterCollection)
        {
            var dictionary = new Dictionary<string, object>();
            var parameters = columnsString.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            // TODO:
            return dictionary;
        }

        public string ExtractInsertObject(string commandText, DbParameterCollection parameterCollection, out object data)
        {
            var tableName = string.Empty;
            data = null;
            var regexTableName = new Regex("INSERT INTO (.*) \\(([\\s\\w\\d,.]*)\\)\\s*VALUES\\s*\\(([\\s\\w\\d,\\@.]*)\\)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (regexTableName.IsMatch(commandText))
            {
                var groups = regexTableName.Match(commandText).Groups;
                tableName = groups[1].Value;
                var dictionary = new Dictionary<string, object>();
                var columnsString = groups[2].Value;
                if (!string.IsNullOrEmpty(columnsString))
                {
                    var columns = columnsString.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var parametersString = groups[3].Value;
                    if (!string.IsNullOrEmpty(parametersString))
                    {
                        var parameters = parametersString.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < columns.Length; i++)
                        {
                            var columnName = columns[i].Trim();
                            var parameterName = parameters[i].Trim().StartsWith(ParameterIdentifying) ? parameters[i].Trim().Substring(1) : parameters[i].Trim();
                            var parameterValue = parameterCollection.Contains(parameterName) ? parameterCollection[parameterName].Value : parameterCollection[ParameterIdentifying + parameterName].Value;
                            dictionary.Add(columnName, parameterValue);
                        }
                    }
                }
                data = dictionary;
            }
            return tableName;
        }

        public string ExtractUpdateObject(string commandText, DbParameterCollection parameterCollection, out object data, out string queryText)
        {
            var tableName = string.Empty;
            queryText = string.Empty;
            data = null;
            var regexTableName = new Regex("UPDATE ([\\w\\d]*) SET(.*) WHERE (.*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (regexTableName.IsMatch(commandText))
            {
                var groups = regexTableName.Match(commandText).Groups;
                tableName = groups[1].Value;
                var columnsString = groups[2].Value;
                if (!string.IsNullOrEmpty(columnsString))
                {
                    data = ExtractColumnsValue(columnsString, parameterCollection);
                }
                queryText = groups[3].Value;
            }
            else
            {
                regexTableName = new Regex("UPDATE ([\\w\\d]*) SET(.*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (regexTableName.IsMatch(commandText))
                {
                    var groups = regexTableName.Match(commandText).Groups;
                    tableName = groups[1].Value;
                    var columnsString = groups[2].Value;
                    if (!string.IsNullOrEmpty(columnsString))
                    {
                        data = ExtractColumnsValue(columnsString, parameterCollection);
                    }
                }
            }
            return tableName;
        }
    }
}
