using UnityEngine;
using System.Collections.Generic;
using TIZSoft.Database.MySQL;

namespace TIZSoft.Database
{
	public enum DatabaseType { SQLite, mySQL }

	[DisallowMultipleComponent]
	public partial class DatabaseManager : BaseDatabaseManager, IAbstractableDatabase
	{
		[Header("Settings")]
		public DatabaseType databaseType = DatabaseType.SQLite;
		[Tooltip("Player data save interval in seconds (0 to disable).")]
		public float saveInterval = 60f;
		[Tooltip("Deleted user prune interval in seconds (0 to disable).")]
		public float deleteInterval = 240f;
		
		public static DatabaseManager singleton;
		
		protected DatabaseType _databaseType = DatabaseType.SQLite;
		
#if wMYSQL
		[Header("Database Layer - mySQL")]
		public MySQLDatabaseLayer databaseLayer;
#else
		[Header("Database Layer - SQLite")]
		public DatabaseLayerSQLite databaseLayer;
#endif
		
		protected const string _defineSQLite 	= "wSQLITE";
		protected const string _defineMySQL 	= "wMYSQL";
		
		void OnValidate()
		{
#if UNITY_EDITOR
			if (databaseType == DatabaseType.mySQL && databaseType != _databaseType)
			{
				EditorTools.RemoveScriptingDefine(_defineSQLite);
				EditorTools.AddScriptingDefine(_defineMySQL);
				_databaseType = databaseType;
			}
			else if (databaseType == DatabaseType.SQLite && databaseType != _databaseType)
			{
				EditorTools.RemoveScriptingDefine(_defineMySQL);
				EditorTools.AddScriptingDefine(_defineSQLite);
				_databaseType = databaseType;
			}
			
			databaseLayer.OnValidate();
			
			this.InvokeInstanceDevExtMethods(nameof(OnValidate));
#endif
		}
		
		void DeleteUsers()
		{
			this.InvokeInstanceDevExtMethods(nameof(DeleteUsers));
			
		}

		void SavePlayers()
		{
			SavePlayers(true);
		}
		
		void SavePlayers(bool online = true)
    	{
			this.InvokeInstanceDevExtMethods(nameof(SavePlayers), online);
    	}

		// Awake
		// Sets the singleton on awake, database can be accessed from anywhere by using it
		// also calls "Init" on database and the databaseLayer to create database and 
		// open connection if
		// that is required

		public override void Awake()
		{
			base.Awake(); // required
			singleton = this;

#if _SERVER
			Init();
#endif

		}


		// Init
		// creates/connects to the database and creates all tables
		// for a multiplayer server based game, this should only be called on the server

		public void Init()
		{

			OpenConnection();

			databaseLayer.Init();

			this.InvokeInstanceDevExtMethods(nameof(Init));

			if (saveInterval > 0)
				InvokeRepeating(nameof(SavePlayers), saveInterval, saveInterval);

			if (deleteInterval > 0)
				InvokeRepeating(nameof(DeleteUsers), deleteInterval, deleteInterval);

			this.InvokeInstanceDevExtMethods(nameof(Init));

		}


		// Destruct
		// closes the connection, cancels saving and updates the checksum (if required),
		// saves all online players and sets them offline.
		// for a multiplayer server based game, this should only be called on the server

		public void Destruct()
		{
			CancelInvoke();
			SavePlayers(false);
			CloseConnection();
		}

		public void OpenConnection()
		{
			databaseLayer.OpenConnection();
			debug.Log("[DatabaseManager] OpenConnection");
		}

		public void CloseConnection()
		{
			databaseLayer.CloseConnection();
			debug.Log("[DatabaseManager] CloseConnection");
		}

		public void CreateTable<T>()
		{
			databaseLayer.CreateTable<T>();
			debug.Log("[DatabaseManager] CreateTable: " + typeof(T));
		}

		public void CreateIndex(string tableName, string[] columnNames, bool unique = false)
		{
			databaseLayer.CreateIndex(tableName, columnNames, unique);
			debug.Log("[DatabaseManager] CreateIndex: " + tableName + " (" + string.Join("_", columnNames) + ")");
		}

		public List<T> Query<T>(string query, params object[] args) where T : new()
		{
			debug.Log("[DatabaseManager] Query: " + typeof(T) + "(" + query + ")");
			return databaseLayer.Query<T>(query, args);
		}

		public void Execute(string query, params object[] args)
		{
			databaseLayer.Execute(query, args);
			debug.Log("[DatabaseManager] Execute: " + query);
		}

		public T FindWithQuery<T>(string query, params object[] args) where T : new()
		{
			debug.Log("[DatabaseManager] FindWithQuery: " + typeof(T) + " (" + query + ")");
			return databaseLayer.FindWithQuery<T>(query, args);
		}


		public void Insert(object obj)
		{
			databaseLayer.Insert(obj);
			debug.Log("[DatabaseManager] Insert: " + obj);
		}

		public void InsertOrReplace(object obj)
		{
			databaseLayer.InsertOrReplace(obj);
			debug.Log("[DatabaseManager] InsertOrReplace: " + obj);
		}

		public void BeginTransaction()
		{
			databaseLayer.BeginTransaction();
			debug.Log("[DatabaseManager] BeginTransaction");
		}

		public void Commit()
		{
			databaseLayer.Commit();
			debug.Log("[DatabaseManager] Commit");
		}
	}
}