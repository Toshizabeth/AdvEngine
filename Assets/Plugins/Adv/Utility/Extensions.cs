using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Adv.Utility
{

	/// <summary>
	/// Array 拡張クラス
	/// </summary>
	public static class ArrayExtensions
	{
		#region 定数, class, enum

		#endregion


		#region public, protected 変数

		public delegate void RefAction<T>(ref T item);
		public delegate void InAction<T>(in T item);

		#endregion


		#region private 変数

		#endregion


		#region プロパティ

		#endregion


		#region コンストラクタ, デストラクタ

		#endregion


		#region public, protected 関数

		public static bool IsNullOrEmpty<T>(this T[] array) =>
			array == null || array.Length <= 0;

		/// <summary>
		/// List.ForEachと同等の機能
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="action"></param>
		public static void ForEach<T>(this T[] array, Action<T> action)
		{
			if (action == null) { throw new ArgumentNullException(nameof(action)); }

			for (int i = 0, count = array.Length; i < count; ++i)
			{
				action?.Invoke(array[i]);
			}
		}

		/// <summary>
		/// refを使った構造体への作用の反映
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="action"></param>
		public static void ForEach<T>(this T[] array, RefAction<T> action)
		{
			if (action == null) { throw new ArgumentNullException(nameof(action)); }

			for (int i = 0, count = array.Length; i < count; ++i)
			{
				action?.Invoke(ref array[i]);
			}
		}

		/// <summary>
		/// inを使った構造体への作用
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="action"></param>
		public static void ForEach<T>(this T[] array, InAction<T> action)
		{
			if (action == null) { throw new ArgumentNullException(nameof(action)); }

			for (int i = 0, count = array.Length; i < count; ++i)
			{
				action?.Invoke(in array[i]);
			}
		}

		/// <summary>
		/// ラムダ式の戻り値を使用して作用の反映
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="action"></param>
		public static void ForEach<T>(this T[] array, Func<T, T> action)
		{
			if (action == null) { throw new ArgumentNullException(nameof(action)); }

			for (int i = 0, count = array.Length; i < count; ++i)
			{
				array[i] = action.Invoke(array[i]);
			}
		}

		#endregion


		#region private 関数

		#endregion
	}

	/// <summary>
	/// Listの拡張メソッド
	/// </summary>
	public static class ListExtensions
	{
		public delegate void RefAction<T>(ref T item);
		public delegate TResult RefFunc<T, TResult>(ref T item);


		/// <summary>
		/// Nullか空の場合true
		/// </summary>
		public static bool IsNullOrEmpty<T>(this IList<T> list) =>
			list == null || list.Count <= 0;

		/// <summary>
		/// refを使用した構造体への作用の反映
		/// </summary>
		public static void ForEach<T>(this IList<T> list, RefAction<T> action)
		{
			if (action == null) { throw new ArgumentNullException(nameof(action)); }

			for (int i = 0; i < list.Count; ++i)
			{
				var item = list[i];
				action?.Invoke(ref item);

				list[i] = item;
			}
		}

		/// <summary>
		/// ラムダ式の戻り値を使用して作用を反映させる
		/// </summary>
		public static void ForEach<T>(this IList<T> list, Func<T, T> func)
		{
			if (func == null) { throw new ArgumentNullException(nameof(func)); }

			for (int i = 0; i < list.Count; ++i)
			{
				list[i] = func(list[i]);
			}
		}

		/// <summary>
		/// 構造体で保存されているリストに対して参照を渡して条件を見て削除する
		/// </summary>
		public static void RemoveOnceWithRef<T>(this IList<T> self, RefFunc<T, bool> func)
		{
			if (func == null) { throw new ArgumentNullException(nameof(func)); }

			for (int i = 0; i < self.Count; ++i)
			{
				var item = self[i];
				if (func(ref item))
				{
					// 削除して終わり
					self.RemoveAt(i);
					return;
				}
			}
		}

		public static void RemoveAllWithRef<T>(this IList<T> self, RefFunc<T, bool> func)
		{
			if (func == null) { throw new ArgumentNullException(nameof(func)); }

			for (int i = 0; i < self.Count; ++i)
			{
				var item = self[i];
				if (func(ref item))
				{
					self.Remove(item);
					--i;
				}
			}
		}

		/// <summary>
		/// リストからランダムな値を取得する
		/// </summary>
		public static T GetRandom<T>(this IList<T> list)
		{
			if (list.IsNullOrEmpty()) return default(T);

			return list[Adv.Utility.Random.Range(list.Count())];
		}

		/// <summary>
		/// 指定した値以外のランダムな値を取得する
		/// </summary>
		public static bool TryGetRandomWithoutValue<T>(this IList<T> self, T value, out T result) where T : IEquatable<T>
		{
			result = default(T);

			if (self.IsNullOrEmpty()) return false;

			if (self.Count == 1)
			{
				var front = self.Front();
				if (front.Equals(value)) return false;

				result = front;
				return true;
			}

			var index = self.IndexOf(value);
			if (index < 0)
			{
				result = GetRandom(self);
				return true;
			}

			var range = Enumerable.Range(0, self.Count)
				.Where(index_ => index_ != index);

			index = range.ElementAt(Adv.Utility.Random.Range(self.Count - 1));
			result = self[index];

			return true;
		}

		/// <summary>
		/// 先頭のデータを返す
		/// </summary>
		public static T Front<T>(this IList<T> self)
		{
			if (self.IsNullOrEmpty()) return default(T);
			return self[0];
		}

		/// <summary>
		/// 末尾のデータを返す
		/// </summary>
		public static T End<T>(this IList<T> self)
		{
			if (self.IsNullOrEmpty()) return default(T);
			return self[self.Count - 1];
		}

		/// <summary>
		/// Listを逆順にループさせる。
		/// reverseと違い、中身が変更された瞬間例外が出るので注意
		/// </summary>
		public static IEnumerable<T> FastReverse<T>(this IList<T> items)
		{
			for (int i = items.Count - 1; i >= 0; i--)
			{
				yield return items[i];
			}
		}
	}
}