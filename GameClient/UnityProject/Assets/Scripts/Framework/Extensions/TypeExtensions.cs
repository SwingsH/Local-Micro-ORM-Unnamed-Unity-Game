using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace TIZSoft.Extensions
{
	public static class TypeExtensions
	{
		static Dictionary<object, object[]> s_attrTable = new Dictionary<object, object[]>();

		public static T GetCustomAttribute<T>(this System.Type t)
		{
			return t.GetCustomAttributes(typeof(T), true).OfType<T>().FirstOrDefault();
		}

		public static T GetAttribute<T>(this System.Type t)
		{
			return (T)GetAttributes(t).FirstOrDefault(x => x is T);
		}

		public static IEnumerable<T> GetAttributes<T>(this System.Type t)
		{
			return GetAttributes(t).Where(x => x is T).Cast<T>();
		}

		public static T GetAttribute<T>(this FieldInfo f)
		{
			return (T)GetAttributes(f).FirstOrDefault(x => x is T);
		}
		public static T GetAttribute<T>(this MethodInfo m)
		{
			return (T)GetAttributes(m).FirstOrDefault(x => x is T);
		}

		static object[] GetAttributes(object o)
		{
			object[] ret = null;
			if (s_attrTable.TryGetValue(o, out ret))
			{
				return ret;
			}
			else
			{
				MemberInfo info = null;
				if (((info = o as FieldInfo) != null)
					|| ((info = o as System.Type) != null)
					|| ((info = o as MemberInfo) != null))
				{
					ret = info.GetCustomAttributes(true);
				}
				s_attrTable[o] = ret;
				return ret;
			}
		}

		public static System.Type GetType(string typeName)
		{
			System.Type type = System.Type.GetType (typeName);

			if (type == null)
			{
				var types = from assembly in System.AppDomain.CurrentDomain.GetAssemblies ()
				            from assemblyType in assembly.GetTypes ()
					            where assemblyType.FullName == typeName
				            select assemblyType;

				type = types.FirstOrDefault ();
			}

			return type;
		}

		public static T GetCustomAttributes<T>(MemberInfo p) where T : Attribute
		{
			T[] attrs = p.GetCustomAttributes(typeof(T), false) as T[];
			if (attrs.Length > 0)
			{
				return attrs[0];
			}
			return default;
		}
	}
}