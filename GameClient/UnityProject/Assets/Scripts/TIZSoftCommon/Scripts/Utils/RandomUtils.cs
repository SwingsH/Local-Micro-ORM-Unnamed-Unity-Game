using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TIZSoft.Utils
{
	public static class RandomUtils
	{
		public static List<T> GenerateRandom<T>(List<T> values,int num=-1)
		{
			List<T> list = new List<T>();
			T tmp;
			int iS;

			for (int N1 = 0; N1 < values.Count; N1++)
			{
				iS = Random.Range(N1, values.Count);
				tmp = values[N1];
				values[N1] = values[iS];
				values[iS] = tmp;
			}

			if (values.Count > 0)
			{
				if(num<0)
					num = Random.Range(0, values.Count);

				for (int i = 0; i <= num; i++)
					list.Add(values[i]);
			}

			return list;
		}
	}
}