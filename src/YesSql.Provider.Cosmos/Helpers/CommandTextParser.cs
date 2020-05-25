using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace YesSql.Provider.Cosmos.Helpers
{
    public class CommandTextParser
    {
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
                            var parameterName = parameters[i].Trim();
                            var parameterValue = parameterCollection[parameterName].Value;
                            dictionary.Add(columnName, parameterValue);
                        }
                    }
                }
                data = dictionary;
            }
            return tableName;
        }

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
    }
}
