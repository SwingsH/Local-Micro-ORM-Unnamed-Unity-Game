using UnityEngine;
using System.Collections.Generic;
using TIZSoft.Utils.Log;

namespace TIZSoft.Database
{

	public partial class DatabaseManager
	{
		//static readonly Utils.Log.Logger logger = LogManager.Default.FindOrCreateCurrentTypeLogger();

		[DevExtMethods("Init")]
		void Init_Example()
		{
			//CreateTable<TableExample>();
			//CreateIndex(nameof(TableExample), new []{"owner", "name", "amount"});
			string query = "SELECT * FROM " + nameof(TableExample);
			IEnumerable<TableExample> result = Query<TableExample>(query);
			foreach (var row in result)
			{
				logger.Log( LogLevel.Info, row.name);
			}
		}
		
		[DevExtMethods("CreateDefaultDataPlayer")]
		void CreateDefaultDataPlayer_Example(GameObject player)
		{

		}
		
		[DevExtMethods("LoadDataPlayerPriority")]
		void LoadDataPlayerPriority_Example(GameObject player)
		{

		}
		
		[DevExtMethods("LoadDataPlayer")]
		void LoadDataPlayer_Example(GameObject player)
		{

		}
		
		[DevExtMethods("SaveDataPlayer")]
		void SaveDataPlayer_Example(GameObject player, bool isOnline)
		{
	   		DeleteDataPlayer_Example(player.name);
		}

	   	[DevExtMethods("DeleteDataPlayer")]
	   	void DeleteDataPlayer_Example(string _name)
	   	{
	   		Execute("DELETE FROM "+nameof(TableExample)+" WHERE owner=?", _name);
	   	}
	}
}
