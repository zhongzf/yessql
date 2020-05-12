using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace YesSql.Provider.Cosmos.Client
{
    public class CosmosParameter : DbParameter
    {
        public CosmosParameter()
        {
        }

        public CosmosParameter(string parameterName, object value)
        {
            ParameterName = parameterName;
            Value = value;
        }

        public override int Size { get; set; }
        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; }
        public override string SourceColumn { get; set; }
        public override object Value { get; set; }
        public override bool SourceColumnNullMapping { get; set; }

        public override void ResetDbType()
        {
            DbType = default(DbType);
        }

        internal void ResetParent()
        {
        }

        internal object CompareExchangeParent(CosmosParameterCollection collection, object value)
        {
            return collection;
        }
    }
}
