using System;
using System.Collections.Generic;
using UnityEngine;
using TIZSoft.Database;
using TIZSoft.Database.MySQL;
using TIZSoft.Database.SQLite;
using TIZSoft.Utils.Log;

[DisallowMultipleComponent]
[System.Serializable]
public class DeployDatabase : MonoBehaviorSingleton<DeployDatabase>
{
    [System.Serializable]
    public class DeploySettings
    {
        [Header("Target Database Type")]
        public DatabaseType databaseType;

        [Header("Database Layer - mySQL")]
        public MySQLDatabaseLayer mysqlLayer;

        [Header("Database Layer - SQLite")]
        public SQLiteDatabaseLayer sqliteLayer;

    }

    static readonly TIZSoft.Utils.Log.Logger logger = LogManager.Default.FindOrCreateLogger<DeployDatabase>();

    [Header("Deploy Settings")]
    [SerializeField]
    public DeploySettings deploySettings;
    private DatabaseManager databaseManager;

    void Start()
    {
        databaseManager = gameObject.AddComponent<DatabaseManager>();
        databaseManager.databaseLayer = deploySettings.mysqlLayer;
        deploySettings.mysqlLayer.FetchSettingFromEditorPrefs();

        //BatchCreateTable();
    }
    public virtual void BatchCreateTable() 
    {
        if (deploySettings == null)
        {
            logger.Log(LogLevel.Error, "OnValidate() argument null.");
            return;
        }

        if (deploySettings.mysqlLayer == null)
        {
            logger.Log(LogLevel.Error, "OnValidate() argument null.");
            return;
        }
        MySQLDatabaseLayer dbal = deploySettings.mysqlLayer;
        dbal.CreateTable<UserSave>();
        dbal.CreateTable<TableExample>();
    }

    void OnValidate()
    {
        if(deploySettings == null)
        {
            logger.Log(LogLevel.Error, "OnValidate() argument null.");
            return;
        }

        if (deploySettings.mysqlLayer == null)
        {
            logger.Log(LogLevel.Error, "OnValidate() argument null.");
            return;
        }

        deploySettings.mysqlLayer.OnValidate();
    }
}
