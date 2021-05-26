
using TIZSoft.Database.MySQL;
using Dapper.Contrib.Extensions;

namespace TIZSoft.Database
{
	[Table("table-example")]
	partial class TableExample
	{
		[PrimaryKey]
		public string 	owner 	{ get; set; }
		public string 	name 	{ get; set; }
		public long 	amount 	{ get; set; }
	}
}
