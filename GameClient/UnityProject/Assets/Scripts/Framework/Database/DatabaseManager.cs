using UnityEngine;
using System.Collections.Generic;
using TIZSoft.Database.MySQL;
using TIZSoft.Database.SQLite;
using TIZSoft.Utils.Log;

namespace TIZSoft.Database
{
	public enum DatabaseType { SQLite, mySQL }

	[DisallowMultipleComponent]
	public partial class DatabaseManager : BaseDatabaseManager, IAbstractableDatabase
	{
		static readonly Utils.Log.Logger logger = Utils.Log.LogManager.Default.FindOrCreateLogger<BaseDatabaseManager>();

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
			
			if(databaseLayer!= null)
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

		public override void Awake()
		{
			base.Awake();
			singleton = this;

#if _SERVER
			Init();
#endif
		}

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

		public void Destruct()
		{
			CancelInvoke();
			SavePlayers(false);
			CloseConnection();
		}

		public void OpenConnection()
		{
			databaseLayer.OpenConnection();
			logger.Log(LogLevel.Info, "[DatabaseManager] OpenConnection");
		}

		public void CloseConnection()
		{
			databaseLayer.CloseConnection();
			logger.Log(LogLevel.Info, "[DatabaseManager] CloseConnection");
		}

		public void CreateTable<T>()
		{
			databaseLayer.CreateTable<T>();
			logger.Log(LogLevel.Info, "[DatabaseManager] CreateTable: " + typeof(T));
		}

		public void CreateIndex(string tableName, string[] columnNames, bool unique = false)
		{
			databaseLayer.CreateIndex(tableName, columnNames, unique);
			logger.Log(LogLevel.Info, "[DatabaseManager] CreateIndex: " + tableName + " (" + string.Join("_", columnNames) + ")");
		}

		public List<T> Query<T>(string query, params object[] args) where T : new()
		{
			logger.Log(LogLevel.Info, "[DatabaseManager] Query: " + typeof(T) + "(" + query + ")");
			return databaseLayer.Query<T>(query, args);
		}

		public IEnumerable<T> Query<T>(string query) where T : new()
		{
			logger.Log(LogLevel.Info, "[DatabaseManager] Query: " + typeof(T) + "(" + query + ")");
			return databaseLayer.Query<T>(query);
		}

		public void Execute(string query, params object[] args)
		{
			databaseLayer.Execute(query, args);
			logger.Log(LogLevel.Info, "[DatabaseManager] Execute: " + query);
		}

		public T FindWithQuery<T>(string query, params object[] args) where T : new()
		{
			logger.Log(LogLevel.Info, "[DatabaseManager] FindWithQuery: " + typeof(T) + " (" + query + ")");
			return databaseLayer.FindWithQuery<T>(query, args);
		}


		public void Insert(object obj)
		{
			databaseLayer.Insert(obj);
			logger.Log(LogLevel.Info, "[DatabaseManager] Insert: " + obj);
		}

		public void InsertOrReplace(object obj)
		{
			databaseLayer.InsertOrReplace(obj);
			logger.Log(LogLevel.Info, "[DatabaseManager] InsertOrReplace: " + obj);
		}

		public void BeginTransaction()
		{
			databaseLayer.BeginTransaction();
			logger.Log(LogLevel.Info, "[DatabaseManager] BeginTransaction");
		}

		public void Commit()
		{
			databaseLayer.Commit();
			logger.Log(LogLevel.Info, "[DatabaseManager] Commit");
		}
	}
}