using System;
using System.Collections.Generic;
using UnityEngine;
using TIZSoft.Database;
using TIZSoft.Database.MySQL;
using TIZSoft.Database.SQLite;
using TIZSoft.Utils.Log;
using TIZSoft.Database.SqlGenerator;
using Dapper;

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

        deploySettings.mysqlLayer.CreateTable<UserSave>();
        deploySettings.mysqlLayer.CreateTable<TableExample>();
        VerifyTable();
    }

    public void VerifyTable()
    {
        SqlGenerator<UserSave> sqlGenerator = new SqlGenerator<UserSave>();
        string sql = string.Empty;

        // Verify INSERT Statement 
        sql = sqlGenerator.GetInsert();
        logger.Debug(sql);
        var param = new DynamicParameters(
                new UserSave
                {
                    Id = 1,
                    Guid = "testguid_1",
                    Name = "Jason Wills",
                    ScreenName = "Jason",
                    CreateDatetime = DateTime.Now,
                    LastLoginDeviceLang = "zh-tw",
                    LastLoginDatetime = DateTime.Now,
                    LastLoginDeviceName = "iphone-w"
                });
        int effectRows = deploySettings.mysqlLayer.RapidExecute<UserSave>(sql, param);
        logger.Debug("INSERT effectRows: " + effectRows.ToString());

        string query = new SqlGenerator<UserSave>().GetInsert();
        effectRows = deploySettings.mysqlLayer.RapidExecute<UserSave>(sql,
            new[] {
                new UserSave
                {
                    Id = 2,
                    Guid = "testguid_2",
                    Name = "Anthony",
                    ScreenName = "Son of Anton",
                    CreateDatetime = DateTime.Now,
                    LastLoginDeviceLang = "zh-tw",
                    LastLoginDatetime = DateTime.Now, 
                    LastLoginDeviceName="bad android"
                },
                new UserSave
                {
                    Id = 3,
                    Guid = "testguid_3",
                    Name = "Bill Russle",
                    ScreenName = "BRus",
                    CreateDatetime = DateTime.Now,
                    LastLoginDeviceLang = "zh-tw",
                    LastLoginDatetime = DateTime.Now,
                    LastLoginDeviceName="bot berry"
                },
                new UserSave
                {
                    Id = 4,
                    Guid = "testguid_4",
                    Name = "IDK",
                    ScreenName = "IDK HAHA",
                    CreateDatetime = DateTime.Now,
                    LastLoginDeviceLang = "zh-tw",
                    LastLoginDatetime = DateTime.Now,
                    LastLoginDeviceName="bad asshole?"
                }
            }
            );
            logger.Debug("INSERT effectRows: " + effectRows.ToString());

        // Verify DELETE Statement 
        sql = sqlGenerator.GetDelete();
        logger.Debug(sql);
        effectRows = deploySettings.mysqlLayer.RapidExecute<UserSave>(sql,
            new[] { new UserSave { Id = 1 }, new UserSave { Id = 3 }, new UserSave { Id = 4 } });
        logger.Debug("DELETE effectRows: " + effectRows.ToString());

        // Verify UPDATE Statement 
        sql = sqlGenerator.GetUpdate();
        logger.Debug(sql);
        effectRows = deploySettings.mysqlLayer.RapidExecute<UserSave>(sql, 
            new UserSave { 
                    Id = 2,
                    Name = "NEW Anthony",
                    ScreenName = "Son of Anton . Type 2 !!",
                    LastLoginDatetime = DateTime.Now,
                    LastLoginDeviceName="good android"
                }
            );
        logger.Debug("UPDATE effectRows: " + effectRows.ToString());

        // Verify SELECT Statement 
        sql = sqlGenerator.GetSelect(new { Id = 2 });
        logger.Debug(sql);
        UserSave result = deploySettings.mysqlLayer.RapidQuerySingle<UserSave>(sql, new UserSave {Id = 2});
        if(result!=null)
            logger.Debug( string.Format(" SELECT Id:{0} Name:{1} LastLoginDeviceName:{2}", result.Id, result.ScreenName, result.LastLoginDeviceName));

        //databaseManager.Query<TableExample>("SELECT * FROM TableExample WHERE owner=?", "name");
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
