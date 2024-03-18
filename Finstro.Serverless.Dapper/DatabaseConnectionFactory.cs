using System.Collections.Generic;
using System.Configuration;using System.Data;using System.Data.Common;
using System.Linq;

namespace Finstro.Serverless.Dapper
{
    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private ConnectionStringSettings _connectionStringSettingKey = new ConnectionStringSettings("ConnectionString", Helper.AppSettings.DataBase.ConnectionString, "MySql.Data.MySqlClient");
        public DatabaseConnectionFactory() { }
        public DatabaseConnectionFactory(ConnectionStringSettings connectionStringSettingKey)
        {
            _connectionStringSettingKey = connectionStringSettingKey;
        }
        public IDbConnection GetConnection()
        {
            return GetConnection(_connectionStringSettingKey);
        }

        public IDbConnection GetConnection(ConnectionStringSettings connectionString)
        {
            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySql.Data.MySqlClient.MySqlClientFactory.Instance);

            // Get the provider invariant names
            IEnumerable<string> invariants = DbProviderFactories.GetProviderInvariantNames(); // => 1 result; 'test'

            // Get a factory using that name
            DbProviderFactory factory = DbProviderFactories.GetFactory(invariants.FirstOrDefault());




            var providerName = connectionString.ProviderName;
            //var factory = DbProviderFactories.GetFactory(providerName);
            var connection = factory.CreateConnection();
            connection.ConnectionString = connectionString.ConnectionString;

            return connection;
        }
    }
    public interface IDatabaseConnectionFactory
    {
        IDbConnection GetConnection();
    }




}
