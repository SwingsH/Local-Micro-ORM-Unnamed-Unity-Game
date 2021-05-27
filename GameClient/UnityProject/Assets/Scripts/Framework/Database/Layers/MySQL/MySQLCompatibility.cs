
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Dapper.Contrib.Extensions;
using TIZSoft.Extensions;

namespace TIZSoft.Database.MySQL
{
	public partial class MySQLCompatibility
	{
		protected Dictionary<int, MySQLTableMapper> tableMaps = new Dictionary<int, MySQLTableMapper>();
		protected Dictionary<int, MySqlParameter[]> mySQLParameters = new Dictionary<int, MySqlParameter[]>();
		protected Dictionary<int, string> mySQLQueries = new Dictionary<int, string>();

		public MySQLTableMapper GetTableMapper<T>()
		{
			string tableName = typeof(T).Name;
			
			tableMaps.TryGetValue(tableName.GetDeterministicHashCode(), out MySQLTableMapper tableMap);
			
			if (tableMap != null)
				return tableMap;
			
			MySQLTableMapper newTableMap = BuildTableMapFromType<T>();
			
			tableMaps.Add(tableName.GetDeterministicHashCode(), newTableMap);
			
			return newTableMap;
		}
		
		public MySQLTableMapper GetTableMapper(object obj)
		{
			string tableName = obj.GetType().Name;
			
			tableMaps.TryGetValue(tableName.GetDeterministicHashCode(), out MySQLTableMapper tableMap);
			
			if (tableMap != null)
			{
				tableMap.UpdateValues(obj);
				return tableMap;
			}
			
			MySQLTableMapper newTableMap = BuildTableMapFromObject(obj);
			tableMaps.Add(tableName.GetDeterministicHashCode(), newTableMap);
			newTableMap.UpdateValues(obj);
			
			return newTableMap;
		}
		
		public MySqlParameter[] GetConvertedParameters(object[] args)
		{
		
			int hash = Tools.GetArrayHashCode(args);
			
			mySQLParameters.TryGetValue(hash, out MySqlParameter[] cachedParameters);
			
			if (cachedParameters != null)
				return cachedParameters;
			
			MySqlParameter[] newParameters = BuildConvertedParameters(args);
			
			mySQLParameters.Add(hash, newParameters);
			
			return newParameters;
			
		}

		public string GetConvertedQuery(string query)
		{
			int hash = query.GetDeterministicHashCode();
			
			mySQLQueries.TryGetValue(hash, out string cachedQuery);
			
			if (cachedQuery != null && !String.IsNullOrWhiteSpace(cachedQuery))
				return cachedQuery;
				
			string newQuery = BuildConvertedQuery(query);
			
			mySQLQueries.Add(hash, newQuery);
		
			return newQuery;
		}
		
		public List<T> ConvertReader<T>(MySQLRowsReader reader)
		{

			if (reader.RowCount == 0)
				return null;
			
			List<T> results = new List<T>();
			
			while (reader.Read())
			{
				MySQLTableMapper map = GetTableMapper<T>();
				
				for (int i = 0; i < map.rows.Length; i++)
				{
					object obj = null;
					
					if (map.rows[i].type == typeof(int))
					{
						obj = reader.GetInt32(map.rows[i].name);
					}
					else if (map.rows[i].type == typeof(bool))
					{
						obj = reader.GetBoolean(map.rows[i].name);
					}		
					else if (map.rows[i].type == typeof(long))
					{
						obj = reader.GetInt64(map.rows[i].name);
					}		
					else if (map.rows[i].type == typeof(string))
					{
						obj = reader.GetString(map.rows[i].name);
					}		
					else if (map.rows[i].type == typeof(DateTime))
					{
						obj = reader.GetDateTime(map.rows[i].name);
					}
					else if (map.rows[i].type == typeof(float))
					{
						obj = reader.GetFloat(map.rows[i].name);
					}
					else if (map.rows[i].type == typeof(double))
					{
						obj = reader.GetDouble(map.rows[i].name);
					}
					
					map.UpdateValue(map.rows[i].name, obj);
				}

				results.Add(map.ToType<T>());
			}
			
			return results;
			
		}
				
		protected MySQLTableMapper BuildTableMapFromType<T>()
		{
			
			bool hasPrimary = false;
			PropertyInfo[] pInfo;
			Type t = typeof(T);
			pInfo = t.GetProperties();

			string tableName = TryGetTableName(typeof(T));
			MySQLTableMapper tableMap = new MySQLTableMapper(t, tableName, pInfo.Length);

			for (int i = 0; i < pInfo.Length; i++)
			{
				tableMap.rows[i] = new TableRowMapper();
				tableMap.rows[i].name = TryGetColumnName(pInfo[i]);
				tableMap.rows[i].type = pInfo[i].PropertyType;
				
				if (IsPK(pInfo[i]) && !hasPrimary)
				{
					tableMap.rows[i].primary = true;
					hasPrimary = true;
				}
			}
			
			return tableMap;
		}
				
		protected MySQLTableMapper BuildTableMapFromObject(object obj)
		{
			bool hasPrimary = false;			
			PropertyInfo[] pInfo;
			Type t = obj.GetType();
			pInfo = t.GetProperties();

			string tableName = TryGetTableName(obj.GetType());
			MySQLTableMapper tableMap = new MySQLTableMapper(t, tableName, pInfo.Length);
			
			for (int i = 0; i < pInfo.Length; i++)
			{
				tableMap.rows[i] = new TableRowMapper();
				tableMap.rows[i].name = TryGetColumnName(pInfo[i]);
				tableMap.rows[i].type = pInfo[i].PropertyType;
				
				if (IsPK(pInfo[i]) && !hasPrimary)
				{
					tableMap.rows[i].primary = true;
					hasPrimary = true;
				}
			}
			
			return tableMap;
		}
		
		protected MySqlParameter[] BuildConvertedParameters(object[] args)
		{
			MySqlParameter[] parameters = new MySqlParameter[args.Length];
			
			for (int i = 0; i < args.Length; i++)
				parameters[i] = new MySqlParameter("@"+i.ToString(), args.GetValue(i));
				
			return parameters;
		
		}
		protected string BuildConvertedQuery(string query)
		{
		
			int count = query.Split('?').Length -1;
			
			for (int i = 0; i < count; i++)
				query = query.ReplaceFirstInstance("?", "@"+i.ToString());
			
			return query;
			
		}
		
        protected bool IsPK (MemberInfo p)
		{
			return p.CustomAttributes.Any (x => x.AttributeType == typeof (PrimaryKeyAttribute));
		}
		
        protected bool IsAutoInc (MemberInfo p)
		{
			return p.CustomAttributes.Any (x => x.AttributeType == typeof (AutoIncrementAttribute));
		}

		/// <summary>
		/// todo : SQLite as similar features about "TableAttribute"
		/// </summary>
		protected string TryGetColumnName(MemberInfo p)
        {
			string result = p.Name;

			ColomnAttribute attr = TypeExtensions.GetCustomAttributes<ColomnAttribute>(p);
			if(attr != default(ColomnAttribute) && attr.ColName != string.Empty)
            {
				result = attr.ColName;
			}
			return result;
		}

		/// <summary>
		/// todo : SQLite as similar features about "TableAttribute"
		/// </summary>
		protected string TryGetTableName(MemberInfo p)
		{
			string result = p.Name;

			TableAttribute attr = TypeExtensions.GetCustomAttributes<TableAttribute>(p);
			if (attr != default(TableAttribute) && attr.Name != string.Empty)
			{
				result = attr.Name;
			}
			return result;
		}
	}
}