using UnityEngine;

namespace TIZSoft.Database
{

	public partial class DatabaseManager
	{
		[DevExtMethods("Init")]
		void Init_Example()
		{
	   		CreateTable<TableExample>();
        	CreateIndex(nameof(TableExample), new []{"owner", "name", "amount"});
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
