using System.Collections.Generic;
using UnityEngine;

namespace GorillaExtensions;

public static class ListExtensions
{
	public static TCol ShuffleIntoCollection<TCol, TVal>(this List<TVal> list) where TCol : ICollection<TVal>, new()
	{
		List<TVal> list2 = new List<TVal>(list);
		TCol result = new TCol();
		int num = list2.Count;
		while (num > 1)
		{
			num--;
			int num2 = Random.Range(0, num);
			List<TVal> list3 = list2;
			int index = num;
			List<TVal> list4 = list2;
			int index2 = num2;
			TVal value = list2[num2];
			TVal value2 = list2[num];
			list3[index] = value;
			list4[index2] = value2;
		}
		foreach (TVal item in list2)
		{
			result.Add(item);
		}
		return result;
	}
}
