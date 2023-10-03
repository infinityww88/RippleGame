using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PatternData", menuName = "Block/PatternData", order = -1)]
public class PatternData : ScriptableObject
{
    public const int MAX_SIZE = 5;
    [SerializeField]
	private ulong data;

    [SerializeField]
    private int row;

	[SerializeField]
    private int col;

    public int Row => row;
    public int Col => col;

    private bool CoordInRange(int row, int col)
    {
        return row >= 0 && row < this.row || col >= 0 && col < this.col;
        //throw new IndexOutOfRangeException($"pattern asset coord [{row}, {col}] out of range [{this.row}, {this.col}]");
    }

	public static bool GetMask(int row, int col, ulong data)
    {
	    return ((1ul << (row * 8 + col)) & data) != 0;
    }

	public static ulong SetMask(int row, int col, ulong data, bool value)
    {
        if (value)
        {
	        data = (1ul << (row * 8 + col)) | data;
        }
        else
        {
	        data = ~(1ul << (row * 8 + col)) & data;
        }
        return data;
    }
    
	public static Tuple<int, int> GetSize(ulong data)
	{
		int row = 0, col = 0;
		for (int i = 0; i < MAX_SIZE; i++) {
			for (int j = 0; j < MAX_SIZE; j++) {
				if (GetMask(i, j, data)) {
					row = Mathf.Max(row, i + 1);
					col = Mathf.Max(col, j + 1);
				}
			}
		}
		return new Tuple<int, int>(row, col);
	}

    public bool this[int row, int col]
    {
        get
        {
            if (CoordInRange(row, col))
            {
                return GetMask(row, col, data);
            }
            return false;
        }

        private set
        {
            if (CoordInRange(row, col))
            {
	            data = SetMask(row, col, data, value);
            }
        }
    }
} 