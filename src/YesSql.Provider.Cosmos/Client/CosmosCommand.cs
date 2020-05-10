using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

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

        private DbParameterCollection _dbParameterCollection = new CosmosParameterCollection();
        protected override DbParameterCollection DbParameterCollection => _dbParameterCollection;

        protected override DbTransaction DbTransaction { get; set; }

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
            throw new NotImplementedException();
        }

        public override object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }
    }
}
