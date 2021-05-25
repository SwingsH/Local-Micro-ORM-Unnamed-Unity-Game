
using TIZSoft;
using TIZSoft.Database;
using TIZSoft.DebugManager;
using UnityEngine;
using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using MySql.Data;
using MySql.Data.MySqlClient;
using SQLite;
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
		
		protected MySQLCompatibility dbCompat = new MySQLCompatibility();
		
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
			MySQLTableMapper tableMap = dbCompat.GetTableMapper<T>();
			
			string primaryKeyString = "";
			
			if (tableMap.HasPrimaryKey)
				primaryKeyString = ", PRIMARY KEY (`"+tableMap.GetPrimaryKey+"`)";
			
			string queryString = "CREATE TABLE IF NOT EXISTS "+tableMap.name+"("+tableMap.RowsToMySQLInsertString+primaryKeyString+") CHARACTER SET="+charset;

			ExecuteNonQuery(connection, null, queryString);
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
			ExecuteNonQuery(connection, null, dbCompat.GetConvertedQuery(query), dbCompat.GetConvertedParameters(args));
		}
		
		public override List<T> Query<T>(string query, params object[] args)
		{
			MySQLRowsReader reader = ExecuteReader(connection, null, dbCompat.GetConvertedQuery(query), dbCompat.GetConvertedParameters(args));
			return dbCompat.ConvertReader<T>(reader);
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
			
			MySQLTableMapper tableMap = dbCompat.GetTableMapper(obj);
			
			string queryString = "INSERT INTO "+tableMap.name+" ("+tableMap.RowsToMySQLString()+") VALUES("+tableMap.RowsToMySQLString("@")+")";

			ExecuteNonQuery(connection,null,  queryString, tableMap.RowsToMySQLParameters);
		}
		
		public override void InsertOrReplace(object obj)
		{
			if (obj == null)
				return;
			
			MySQLTableMapper tableMap = dbCompat.GetTableMapper(obj);
			
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
			address 	= EditorTools.EditorPrefsUpdateString(Constants.EditorPrefsMySQLAddress, address);
			port 		= (uint)EditorTools.EditorPrefsUpdateInt(Constants.EditorPrefsMySQLPort, (int)port);
			username 	= EditorTools.EditorPrefsUpdateString(Constants.EditorPrefsMySQLUsername, username);
			password 	= EditorTools.EditorPrefsUpdateString(Constants.EditorPrefsMySQLPassword, password);
			dbName 		= EditorTools.EditorPrefsUpdateString(Constants.EditorPrefsMySQLDatabase, dbName);
			charset		= EditorTools.EditorPrefsUpdateString(Constants.EditorPrefsMySQLCharset, charset);
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
		
		
		// NewConnection
		
		MySqlConnection NewConnection()
        {
            return new MySqlConnection(GetConnectionString);
        }
		
		
		// ExecuteInsertData
		
        long ExecuteInsertData(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            connection.Open();
            long result = ExecuteInsertData(connection, null, sql, args);
            connection.Close();
            return result;
        }
		
		
		// ExecuteInsertData
		
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
		
		
		// ExecuteNonQuery
		
        int ExecuteNonQuery(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            connection.Open();
            int result = ExecuteNonQuery(connection, null, sql, args);
            connection.Close();
            return result;
        }
		
		
		// ExecuteNonQuery
		
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
		
		
		// ExecuteScalar
		
        object ExecuteScalar(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            connection.Open();
            object result = ExecuteScalar(connection, null, sql, args);
            connection.Close();
            return result;
        }
		
		
		// ExecuteScalar
		
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
		
		
		// ExecuteReader
		
        MySQLRowsReader ExecuteReader(string sql, params MySqlParameter[] args)
        {
            MySqlConnection connection = NewConnection();
            connection.Open();
            MySQLRowsReader result = ExecuteReader(connection, null, sql, args);
            connection.Close();
            return result;
        }
		
		
		// ExecuteReader
		
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

// =======================================================================================