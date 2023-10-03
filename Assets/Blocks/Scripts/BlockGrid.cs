using ScriptableObjectArchitecture;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class BlockGrid : MonoBehaviour
{
    public GameObjectGameEvent eventGameObjectOnPatternRelease;
    public GameObjectGameEvent eventGameObjectOnPatternPreview;
    public int Row;
    public int Col;
    public GameObject blockPrefab;
    private RectInt gridRect;
    private bool[] gridMask;
    private Transform blockRoot;
    public Color32 hideColor;
	public LevelLogic levelLogic;
    

    private void Start()
	{
        gridRect = new RectInt(0, 0, Col, Row);
        gridMask = new bool[Row * Col];
        previewGridMask = new bool[Row * Col];
        blockRoot = transform.GetChild(0);
    }

    public Vector2 Center
    {
        get
        {
            return new Vector2((Col-1)/2.0f, -(Row-1)/2.0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector2 leftTop = (Vector2)transform.position - Center - new Vector2(0.5f, -0.5f);
        Gizmos.DrawSphere(leftTop, 0.2f);
    }

    public Vector2Int GetGridCoord(Vector2 worldPosition)
    {
        Vector2 leftTop = (Vector2)transform.position - Center - new Vector2(0.5f, -0.5f);
        Vector2 diff = worldPosition - leftTop;
        Vector2Int coord = new Vector2Int(Mathf.FloorToInt(diff.x), -Mathf.CeilToInt(diff.y));
        return coord;
    }

    private int Index(Vector2Int coord)
    {
        return Index(coord.x, coord.y);
    }
    
    private int Index(int x, int y)
    {
        return y * Col + x;
    }
    
    private Block GetBlock(Vector2Int coord)
    {
        return blockRoot.GetChild(Index(coord)).gameObject.GetComponent<Block>();
    }

    [ExecuteInEditMode]
    private void CreateGrid()
    {
        if (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        blockRoot = new GameObject("BlockRoot").transform;
        blockRoot.transform.SetParent(transform);
        blockRoot.transform.SetAsFirstSibling();
        blockRoot.transform.localPosition = Vector3.zero;
        for (int i = 0; i < Row; i++)
        {
            for (int j =  0; j < Col; j++)
            {
                GameObject child = Instantiate(blockPrefab, blockRoot.transform);
                child.GetComponent<Block>().Color = hideColor;
                child.transform.localPosition = new Vector2(j, -i) - Center;
            }
        }
    }

    private void EraseRows(List<int> rows)
    {
        rows.ForEach(EraseRow);
    }
    private void EraseCols(List<int> cols)
    {
        cols.ForEach(EraseCol);
    }

    private void EraseRow(int row)
	{
		ForEachRowBlocks(row, coord =>
		{
			Block b = GetBlock(coord);
			b.Color = hideColor;
			gridMask[Index(coord)] = false;
		});
    }
    private void EraseCol(int col)
	{
		ForEachColBlocks(col, coord =>
		{
			Block b = GetBlock(coord);
			b.Color = hideColor;
			gridMask[Index(coord)] = false;
		});
    }

    private List<int> GetFullRow(bool[] mask)
    {
        List<int> rows = new List<int>();
        for (int i = 0; i < Row; i++)
        {
            bool full = true;
            for (int j = 0; j < Col; j++)
            {
                if (!mask[Index(j, i)])
                {
                    full = false;
                    break;
                }
            }
            if (full)
            {
                rows.Add(i);
            }
        }
        return rows;
    }
    private List<int> GetFullCol(bool[] mask)
    {
        List<int> cols = new List<int>();
        for (int j = 0; j < Col; j++)
        {
            bool full = true;
            for (int i = 0; i < Row; i++)
            {
                if (!mask[Index(j, i)])
                {
                    full = false;
                    break;
                }
            }
            if (full)
            {
                cols.Add(j);
            }
        }
        return cols;
    }

    private void OnEnable()
    {
        eventGameObjectOnPatternRelease.AddListener(OnPatternRelease);
        eventGameObjectOnPatternPreview.AddListener(OnPatternPreview);

    }

    private void OnDisable()
    {
        eventGameObjectOnPatternRelease.RemoveListener(OnPatternRelease);
        eventGameObjectOnPatternPreview.RemoveListener(OnPatternPreview);
    }

    private bool PatternInGrid(Vector2Int start, BlockPattern bp)
    {
        Vector2Int end = start + new Vector2Int(bp.Col-1, bp.Row-1);
        return gridRect.Contains(start) && gridRect.Contains(end);
    }

	private IEnumerable<Tuple<Vector2Int, Vector2Int>> PatternCoordsOnGrid(Vector2Int start, BlockPattern bp)
    {
        if (PatternInGrid(start, bp))
        {
        	foreach (Tuple<int, int> e in BlockUtils.Enumerate2d(bp.Row, bp.Col)) {
        		int i = e.Item1, j = e.Item2;
        		if (bp.IsSet(i, j)) {
        			yield return new Tuple<Vector2Int, Vector2Int>(
	        			new Vector2Int(start.x + j, start.y + i),
	        			new Vector2Int(j, i));
        		}
        	}
        }
    }
    
    public bool AvailableForPattern(BlockPattern bp)
	{
		return BlockUtils.Enumerate2d(Row, Col).Any(t => AvailableForPattern(new Vector2Int(t.Item2, t.Item1), bp));
    }

    private bool AvailableForPattern(Vector2Int start, BlockPattern bp)
    {
        if (PatternInGrid(start, bp))
        {
	        return PatternCoordsOnGrid(start, bp).All(t => !gridMask[Index(t.Item1)]);
        }
        return false;
    }

    private void ApplyPattern(Vector2Int start, BlockPattern bp)
    {
	    PatternCoordsOnGrid(start, bp).ForEach(t =>
	    {
		    Vector2Int coord = t.Item1;
            Block b = GetBlock(coord);
            b.Color = bp.BlockColor;
            gridMask[Index(coord)] = true;
        });
	    previewRows.ForEach(EraseRow);
	    previewCols.ForEach(EraseCol);
    }

    private void PreviewPattern(Vector2Int start, BlockPattern bp)
    {
        if (!AvailableForPattern(start, bp))
        {
            return;
        }
        Array.Copy(gridMask, previewGridMask, gridMask.Length);
	    PatternCoordsOnGrid(start, bp).ForEach(t =>
	    {
		    Vector2Int coord = t.Item1;
            int index = Index(coord);
            GameObject o = blockRoot.transform.GetChild(index).gameObject;
            Block b = o.GetComponent<Block>();
            Color32 c = bp.BlockColor;
            c.a = 64;
            b.Color = c;
            previewBlocks.Add(coord);
            previewGridMask[index] = true;
        });
        previewRows = GetFullRow(previewGridMask);
        previewCols = GetFullCol(previewGridMask);
        previewRows.ForEach(row => {
            ForEachRowBlocks(row, coord =>
            {
                if (gridMask[Index(coord)])
                {
                    Block b = GetBlock(coord);
                    b.Hint(bp.BlockColor);
                }
            });
        });
        previewCols.ForEach(col =>
        {
            ForEachColBlocks(col, coord =>
            {
                if (gridMask[Index(coord)])
                {
                    Block b = GetBlock(coord);
                    b.Hint(bp.BlockColor);
                }
            });
        });
    }

    private void ForEachRowBlocks(int row, Action<Vector2Int> action)
    {
        for (int i = 0; i < Col; i++)
        {
            action(new Vector2Int(i, row));
        }
    }

    private void ForEachColBlocks(int col, Action<Vector2Int> action)
    {
        for (int i = 0; i < Row; i++)
        {
            action(new Vector2Int(col, i));
        }
    }

    private void UnPreviewPattern()
    {
        previewPattern = null;
        previewRows.ForEach(row => {
            ForEachRowBlocks(row, coord =>
            {
                if (gridMask[Index(coord)])
                {
                    Block b = GetBlock(coord);
                    b.UnHint();
                }
            });
        });
        previewCols.ForEach(col =>
        {
            ForEachColBlocks(col, coord =>
            {
                if (gridMask[Index(coord)])
                {
                    Block b = GetBlock(coord);
                    b.UnHint();
                }
            });
        });
        previewBlocks.ForEach(c =>
        {
            Block b = GetBlock(c);
            b.Color = hideColor;
        });
        previewBlocks.Clear();
    }

    private Vector2Int previewStartCoord;
    private BlockPattern previewPattern;
    private List<Vector2Int> previewBlocks = new List<Vector2Int>();
    private List<int> previewRows = new List<int>();
    private List<int> previewCols = new List<int>();
    private bool[] previewGridMask;

    private void OnPatternPreview(GameObject pattern)
    {
        BlockPattern bp = pattern.GetComponent<BlockPattern>();
        Vector2 worldPos = bp.GetTopLeftPos();
        Vector2Int coord = GetGridCoord(worldPos);
        if (coord == previewStartCoord && previewPattern == pattern)
        {
            return;
        }
        UnPreviewPattern();
        PreviewPattern(coord, bp);
    }

    private void OnPatternRelease(GameObject pattern)
    {
        BlockPattern bp = pattern.GetComponent<BlockPattern>();
        Vector2 worldPos = bp.GetTopLeftPos();
        Vector2Int coord = GetGridCoord(worldPos);
        UnPreviewPattern();
        if (AvailableForPattern(coord, bp))
        {
            ApplyPattern(coord, bp);
            levelLogic.ApplyPattern(bp.gameObject);
            Destroy(pattern);
        }
        else
        {
            levelLogic.CancelPattern(bp.gameObject);
        }
    }
}
