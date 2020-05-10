using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace YesSql.Provider.Cosmos.Client
{
    public class CosmosTransaction : DbTransaction
    {
        private DbConnection _dbConnection;
        protected override DbConnection DbConnection => _dbConnection;

        private IsolationLevel _isolationLevel;
        public override IsolationLevel IsolationLevel => _isolationLevel;

        public CosmosTransaction(DbConnection dbConnection, IsolationLevel isolationLevel)
        {
            _dbConnection = dbConnection;
            _isolationLevel = isolationLevel;
        }

        public override void Commit()
        {
        }

        public override void Rollback()
        {
        }
    }
}
