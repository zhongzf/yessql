using System;
using System.Collections.Generic;
using System.Text;
using YesSql.Sql;

namespace YesSql.Provider.Cosmos
{
    public class CosmosCommandInterpreter : BaseCommandInterpreter
    {
        public CosmosCommandInterpreter(ISqlDialect dialect) : base(dialect)
        {
        }
    }
}
