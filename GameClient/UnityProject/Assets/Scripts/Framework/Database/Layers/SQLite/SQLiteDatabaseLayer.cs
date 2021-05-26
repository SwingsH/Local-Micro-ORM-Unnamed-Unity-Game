
using TIZSoft;
using TIZSoft.Database;
using TIZSoft.DebugManager;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using SQLite;

namespace TIZSoft.Database.SQLite
{
	[System.Serializable]
	public partial class SQLiteDatabaseLayer : DatabaseAbstractionLayer
	{
		[Header("Options")]
		public string databaseName 	= "Database.sqlite";
		[Tooltip("Launch automatically when the game is started (recommended for Single-Player games).")]
		public bool initOnAwake;
		[Tooltip("Compares the hash of the database with player prefs to prevent cheating.")]
		public bool checkIntegrity;
		
		protected 			SQLiteConnection 	connection = null;
		protected static 	string 				_dbPath = "";

		public override void Init()
		{
			if (!initOnAwake) return;
			OpenConnection();
		}

		public override void OpenConnection()
		{
			if (connection != null) return;
			
			_dbPath = Tools.GetPath(databaseName);
			
			if (File.Exists(_dbPath) && checkIntegrity && Tools.GetChecksum(_dbPath) == false) //not recommended on very large files
			{
				debug.LogWarning("[DatabaseManager] Database file is corrupted!");
				File.Delete(_dbPath);// deletes the file, a fresh database file is re-created thereafter
			}
			connection = new SQLiteConnection(_dbPath);
		}

		public override void CloseConnection()
		{
			connection?.Close();
		
			if (checkIntegrity)
				Tools.SetChecksum(_dbPath);
		}
				
		public override void CreateTable<T>()
		{
			connection.CreateTable<T>();
		}
		
		public override void CreateIndex(string tableName, string[] columnNames, bool unique = false)
		{
			connection.CreateIndex(tableName, columnNames, unique);
		}
		
		public override List<T> Query<T>(string query, params object[] args)
		{
			return connection.Query<T>(query, args);
		}

		public override void Execute(string query, params object[] args)
		{
			connection.Execute(query, args);
		}
		
		public override T FindWithQuery<T>(string query, params object[] args)
		{
			return connection.FindWithQuery<T>(query, args);
		}
		
		public override void Insert(object obj)
		{
			connection.Insert(obj);		
		}
		
		public override void InsertOrReplace(object obj)
		{
			connection.InsertOrReplace(obj);		
		}
		
		public override void BeginTransaction()
		{
			connection.BeginTransaction();		
		}
		
		public override void Commit()
		{
			connection.Commit();
		}
		
		public override void OnValidate()
		{
			
		}
	}
}