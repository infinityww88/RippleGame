using System;
using System.Collections.Generic;

public static class BlockUtils
{
    public static void ForEach2d<T>(T[,] array, Action<int, int, T> action)
	{
		foreach (Tuple<int, int> t in Enumerate2d(array.GetLength(0), array.GetLength(1))) {
			int i = t.Item1, j = t.Item2;
			action(i, j, array[i, j]);
		}
	}
    
	public static void ForEach2d(int row, int col, Action<int, int> action) {
		foreach (Tuple<int, int> t in Enumerate2d(row, col)) {
			action(t.Item1, t.Item2);
		}
	}
    
	public static IEnumerable<Tuple<int, int>> Enumerate2d(int row, int col) {
		for (int i = 0; i < row; i++) {
			for (int j = 0; j < col; j++) {
				yield return new Tuple<int, int>(i, j);
			}
		}
	}
	
	public static IEnumerable<T> Enumerate2d<T>(int row, int col, Func<int, int, T> provider) {
		for (int i = 0; i < row; i++) {
			for (int j = 0; j < col; j++) {
				yield return provider(i, j);
			}
		}
	}
	
	public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action) {
		foreach (T v in collection) {
			action(v);
		}
	}

}