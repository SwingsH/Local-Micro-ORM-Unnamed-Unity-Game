
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;
using TIZSoft.Utils.Log;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TIZSoft.Database.MySQL
{
	[System.Serializable]
	public partial class MySQLDatabaseLayer : DatabaseAbstractionLayer
	{
		[Header("Settings")]
		[Tooltip("Default: 127.0.0.1")]
        public string address;
        [Tooltip("Default: 3306")]
        public uint port;
        [Tooltip("Default: root")]
        public string username;
        [Tooltip("Default: password")]
        public string password;
        [Tooltip("Default: mysqldatabase")]
        public string dbName;
        [Tooltip("Default: utf8mb4")]
		public string charset;
		
		protected MySqlConnection connection = null;
		protected string connectionString = null;
		
		protected MySQLCompatibility mysqlCompat = new MySQLCompatibility();
		static readonly Utils.Log.Logger logger = LogManager.Default.FindOrCreateLogger<DeployDatabase>();

		public override void Init()
		{
		}
		
		public override void OpenConnection()
		{
			if (connection == null)
				connection = NewConnection();
				
			connection.Open();
		}
		
		public override void CloseConnection()
		{
			if (connection != null)
				connection.Close();
		}
		
		public override void CreateTable<T>()
		{
			MySQLTableMapper tableMap = mysqlCompat.GetTableMapper<T>();
			
			string primaryKeyString = "";
			
			if (tableMap.HasPrimaryKey)
				primaryKeyString = ", PRIMARY KEY (`"+tableMap.GetPrimaryKey+"`)";
			
			string queryString = "CREATE TABLE IF NOT EXISTS "+tableMap.name+"("+tableMap.RowsToMySQLInsertString+primaryKeyString+") CHARACTER SET="+charset;

			ExecuteNonQuery(connection, null, queryString);
			logger.Debug(queryString);
		}

		public override void CreateIndex(string tableName, string[] columnNames, bool unique = false)
		{
			string indexName = tableName + "_" + string.Join ("_", columnNames);
			
			if (ExecuteScalar(connection, null, "SELECT COUNT(1) "+indexName+" FROM "+tableName) == null)
			{
				string isUnique = unique ? "UNIQUE" : "";
				string queryString = "CREATE INDEX "+indexName+" "+isUnique+" ON "+tableName+" ("+string.Join (",", columnNames)+")";
				ExecuteNonQuery(connection, null, queryString);
			}
		}
		
		public override void Execute(string query, params object[] args)
		{
			ExecuteNonQuery(connection, null, mysqlCompat.GetConvertedQuery(query), mysqlCompat.GetConvertedParameters(args));
		}
		
		public override List<T> Query<T>(string query, params object[] args)
		{
			MySQLRowsReader reader = ExecuteReader(connection, null, mysqlCompat.GetConvertedQuery(query), mysqlCompat.GetConvertedParameters(args));
			return mysqlCompat.ConvertReader<T>(reader);
		}

		/// <summary>
		/// Using Dapper, todo: append Query functions with Dapper
		/// </summary>
		public IEnumerable<T> RapidQuery<T>(string query, object args)
		{
			IDbConnection conn = NewConnection();
			conn.Open();
			return conn.Query<T>(query, args);
		}

		public T RapidQuerySingle<T>(string query, object args)
		{
			IDbConnection conn = NewConnection();
			conn.Open();
			return conn.QuerySingleOrDefault<T>(query, args);
		}

		public int RapidExecute<T>(string query, object args)
        {
			IDbConnection conn = NewConnection();
			conn.Open();
			return conn.Execute(query, args);
		}

		public override T FindWithQuery<T>(string query, params object[] args)
		{
			List<T> list = Query<T>(query, args);
			
			if (list == null)
				return default(T);
			
			return list.FirstOrDefault();
		}
		
		public override void Insert(object obj)
		{
			if (obj == null)
				return;
			
			MySQLTableMapper tableMap = mysqlCompat.GetTableMapper(obj);
			
			string queryString = "INSERT INTO "+tableMap.name+" ("+tableMap.RowsToMySQLString()+") VALUES("+tableMap.RowsToMySQLString("@")+")";

			ExecuteNonQuery(connection,null,  queryString, tableMap.RowsToMySQLParameters);
		}
		
		public override void InsertOrReplace(object obj)
		{
			if (obj == null)
				return;
			
			MySQLTableMapper tableMap = mysqlCompat.GetTableMapper(obj);
			
			string queryString = "REPLACE INTO "+tableMap.name+" ("+tableMap.RowsToMySQLString()+") VALUES("+tableMap.RowsToMySQLString("@")+")";

			ExecuteNonQuery(connection,null,  queryString, tableMap.RowsToMySQLParameters);
		}

		public override void BeginTransaction()
		{
			ExecuteNonQuery("START TRANSACTION");
		}
		
		public override void Commit()
		{
			ExecuteNonQuery("COMMIT");
		}
	
		public override void OnValidate()
		{
#if UNITY_EDITOR
			address 	= EditorTools.EditorPrefsUpdateString(Constants.EDITOR_PREFS_MYSQL_ADDRESS, address);
			port 		= (uint)EditorTools.EditorPrefsUpdateInt(Constants.EDITOR_PREFS_MYSQL_PORT, (int)port);
			username 	= EditorTools.EditorPrefsUpdateString(Constants.EDITOR_PREFS_MYSQL_USERNAME, username);
			password 	= EditorTools.EditorPrefsUpdateString(Constants.EDITOR_PREFS_MYSQL_PASSWORD, password);
			dbName 		= EditorTools.EditorPrefsUpdateString(Constants.EDITOR_PREFS_MYSQL_DATABASE, dbName);
			charset		= EditorTools.EditorPrefsUpdateString(Constants.EDITOR_PREFS_MYSQL_CHARSET, charset);
#endif
		}

		public void FetchSettingFromEditorPrefs()
        {
#if UNITY_EDITOR
			address = EditorPrefs.GetString(Constants.EDITOR_PREFS_MYSQL_ADDRESS, address);
			port = (uint)EditorPrefs.GetInt(Constants.EDITOR_PREFS_MYSQL_PORT, (int)port);
			username = EditorPrefs.GetString(Constants.EDITOR_PREFS_MYSQL_USERNAME, username);
			password = EditorPrefs.GetString(Constants.EDITOR_PREFS_MYSQL_PASSWORD, password);
			dbName = EditorPrefs.GetString(Constants.EDITOR_PREFS_MYSQL_DATABASE, dbName);
			charset = EditorPrefs.GetString(Constants.EDITOR_PREFS_MYSQL_CHARSET, charset);
#endif
		}

		string GetConnectionString
		{
			get
			{
				if (connectionString == null)
				{
					MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
					{
						Server 			= string.IsNullOrWhiteSpace(address) 	? "127.0.0.1" 	: address,
						Database 		= string.IsNullOrWhiteSpace(dbName) 	? "database" 	: dbName,
						UserID 			= string.IsNullOrWhiteSpace(username) 	? "root" 		: username,
						Password 		= string.IsNullOrWhiteSpace(password) 	? "password" 	: password,
						Port 			= port,
						CharacterSet 	= string.IsNullOrWhiteSpace(charset) 	? "utf8mb4" 	: charset
					};
					connectionString = connectionStringBuilder.ConnectionString;
				}
				return connectionString;
			}
		}
		
		MySqlConnection NewConnection()
        {
            return new MySqlConnection(GetConnectionString);
        }
		
        long ExecuteInsertData(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            connection.Open();
            long result = ExecuteInsertData(connection, null, sql, args);
            connection.Close();
            return result;
        }
		
        long ExecuteInsertData(MySqlConnection connection, MySqlTransaction transaction, string sql, params MySqlParameter[] args)
        {
            bool createLocalConnection = false;
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                connection.Open();
                createLocalConnection = true;
            }
            long result = 0;
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                cmd.ExecuteNonQuery();
                result = cmd.LastInsertedId;
            }
            if (createLocalConnection)
                connection.Close();
            return result;
        }
		
        int ExecuteNonQuery(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            connection.Open();
            int result = ExecuteNonQuery(connection, null, sql, args);
            connection.Close();
            return result;
        }
		
        int ExecuteNonQuery(MySqlConnection connection, MySqlTransaction transaction, string sql, params MySqlParameter[] args)
        {
            bool createLocalConnection = false;
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                connection.Open();
                createLocalConnection = true;
            }
            int numRows = 0;
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                numRows = cmd.ExecuteNonQuery();
            }
            if (createLocalConnection)
                connection.Close();
            return numRows;
        }
		
        object ExecuteScalar(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            connection.Open();
            object result = ExecuteScalar(connection, null, sql, args);
            connection.Close();
            return result;
        }
		
        object ExecuteScalar(MySqlConnection connection, MySqlTransaction transaction, string sql, params MySqlParameter[] args)
        {
            bool createLocalConnection = false;
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                connection.Open();
                createLocalConnection = true;
            }
            object result;
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                result = cmd.ExecuteScalar();
            }
            if (createLocalConnection)
                connection.Close();
            return result;
        }
		
        MySQLRowsReader ExecuteReader(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            connection.Open();
            MySQLRowsReader result = ExecuteReader(connection, null, sql, args);
            connection.Close();
            return result;
        }
		
        MySQLRowsReader ExecuteReader(MySqlConnection connection, MySqlTransaction transaction, string sql, params MySqlParameter[] args)
        {
            bool createLocalConnection = false;
            if (connection == null)
            {
                connection = NewConnection();
                transaction = null;
                connection.Open();
                createLocalConnection = true;
            }
            MySQLRowsReader result = new MySQLRowsReader();
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                foreach (MySqlParameter arg in args)
                {
                    cmd.Parameters.Add(arg);
                }
                MySqlDataReader dataReader = cmd.ExecuteReader();
                result.Init(dataReader);
                dataReader.Close();
            }
            if (createLocalConnection)
                connection.Close();
            return result;
        }
	}
}
