using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using YesSql.Provider.Cosmos.Helpers;

namespace YesSql.Provider.Cosmos.Client
{
    public class CosmosCommand : DbCommand
    {
        public override string CommandText { get; set; }
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection DbConnection { get; set; }

        protected override DbParameterCollection DbParameterCollection { get; } = new CosmosParameterCollection();

        protected override DbTransaction DbTransaction { get; set; }

        public CosmosExecutor CosmosExecutor { get; set; }

        public CommandTextParser Parser { get; set; } = new CommandTextParser();

        public override void Cancel()
        {
        }

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter()
        {
            return new CosmosParameter();
        }


        public override int ExecuteNonQuery()
        {
            if(CommandText.StartsWith("alter ", StringComparison.OrdinalIgnoreCase) || CommandText.StartsWith("create ", StringComparison.OrdinalIgnoreCase))
            {
                // TODO: create table
                return 1;
            }
            else if(CommandText.StartsWith("insert into ", StringComparison.OrdinalIgnoreCase))
            {
                var tableName = Parser.ExtractInsertObject(CommandText, DbParameterCollection, out object data);
                if(!string.IsNullOrEmpty(tableName) && data != null)
                {
                    var result = CosmosExecutor.CreateAsync(tableName, data).GetAwaiter().GetResult();
                    return 1;
                }    
            }
            else if(CommandText.StartsWith("update ", StringComparison.OrdinalIgnoreCase))
            {
                var tableName = Parser.ExtractUpdateObject(CommandText, DbParameterCollection, out object data, out string queryText);
                if(!string.IsNullOrEmpty(tableName) && data != null)
                {
                    var result = CosmosExecutor.UpdateAsync(tableName, data, queryText).GetAwaiter().GetResult();
                    return (int)result;
                }
            }
            return 0;
        }

        public override object ExecuteScalar()
        {
            var reader = ExecuteReader();
            if (reader.Read())
            {
                try
                {
                    return reader.GetValue(0);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return null;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            var tableName = Parser.ExtractTableName(CommandText);
            return new CosmosDataReader { FeedIteratorReader = new FeedIteratorReader { FeedIterator = CosmosExecutor.Query(tableName, this.CommandText, this.Parameters, out string queryText), CommandText = this.CommandText, QueryText = queryText } };
        }
    }
}
