using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace YesSql.Provider.Cosmos.Client
{
    public class CosmosConnection : DbConnection
    {
        public override string ConnectionString { get; set; }

        private string _database;
        public override string Database => _database;

        private string _dataSource = string.Empty;
        public override string DataSource => _dataSource;

        private string _serverVersion = "v3";
        public override string ServerVersion => _serverVersion;

        private ConnectionState _state;
        public override ConnectionState State => _state;

        public override void ChangeDatabase(string databaseName)
        {
            _database = databaseName;
        }

        private CosmosClient _client;

        public override void Open()
        {
            _client = new CosmosClient(ConnectionString);
            _state = ConnectionState.Open;
        }

        public override void Close()
        {
            _client.Dispose();
            _client = null;
            _state = ConnectionState.Closed;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new CosmosTransaction(this, isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            return new CosmosCommand { Connection = this };
        }
    }
}
