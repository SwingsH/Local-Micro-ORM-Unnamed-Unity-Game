using System;

namespace TIZSoft.Database.MySQL
{
	public class TableRowMapper
	{
		public string name;
		public Type type;
		public object value;
		public bool primary;

		const string COL_TYPE_INT = " INT";
		const string COL_TYPE_BOOL = " BOOLEAN";
		const string COL_TYPE_LONG = " BIGINT";
		const string COL_TYPE_STRING = " VARCHAR(64)";
		const string COL_TYPE_DATETIME = " DATETIME";
		const string COL_TYPE_FLOAT = " FLOAT";
		const string COL_TYPE_DOUBLE = " DOUBLE";

		public string ToMySQLString
		{
			get
			{
				if (type == typeof(int))
				{
					return "`" + name + "`" + COL_TYPE_INT;
				}
				else if (type == typeof(bool))
				{
					return "`" + name + "`" + COL_TYPE_BOOL;
				}
				else if (type == typeof(long))
				{
					return "`" + name + "`" + COL_TYPE_LONG;
				}
				else if (type == typeof(string))
				{
					return "`" + name + "`" + COL_TYPE_STRING;
				}
				else if (type == typeof(DateTime))
				{
					return "`" + name + "`" + COL_TYPE_DATETIME;
				}
				else if (type == typeof(float))
				{
					return "`" + name + "`" + COL_TYPE_FLOAT;
				}
				else if (type == typeof(double))
				{
					return "`" + name + "`" + COL_TYPE_DOUBLE;
				}

				return "";
			}
		}
	}
}