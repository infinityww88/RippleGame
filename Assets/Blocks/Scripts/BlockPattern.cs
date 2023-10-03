using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockPattern : MonoBehaviour
{
	private PatternData patternData;
	public float m_score;
	private Block[,] blocks;

    void Start()
    {
	    //blocksRoot = new GameObject("blocksRoot").transform;
	    //blocksRoot.SetParent(transform, false);
	    //Debug.Log(patternData);
	    //Debug.Log(Row + ", " + Col);
	    //GeneratePattern();
    }

    private Color color = Color.white;
    public Color32 BlockColor
    {
        get
        {
            return color;
        }
        set
        {
            for (int i = 0; i < blocksRoot.childCount; i++)
            {
                blocksRoot.GetChild(i).GetComponent<SpriteRenderer>().color = value;
                color = value;
            }
        }
    }

    public GameObject blockPrefab;

    [SerializeField]
    private Transform blocksRoot;

	public int Row => patternData.Row;
	public int Col => patternData.Col;

    public void SetSmall()
    {
        blocksRoot.localScale = Vector3.one / 2;
    }

    public void SetNormal()
    {
        blocksRoot.localScale = Vector3.one;
    }

    public bool IsSet(int row, int col)
    {
	    return patternData[row, col];
    }
    
    private Vector3 Center
    {
        get
	    {
            return new Vector3((Col - 1) / 2.0f, -(Row - 1) / 2.0f);
        }
    }

    public Vector3 GetTopLeftPos()
    {
        return transform.position - Center;
    }
    
	public Block GetBlock(int row, int col) {
		return blocks[row, col];
	}

	public void GeneratePattern(PatternData patternData)
	{
		this.patternData = patternData;
		blocks = new Block[patternData.Row, patternData.Col];
		Debug.Log($"generate pattern {patternData}");
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0;  j < Col; j++)
            {
                if (IsSet(i, j))
                {
                    GameObject childBlock = Instantiate(blockPrefab, blocksRoot);
                    childBlock.transform.SetParent(blocksRoot);
                    childBlock.transform.localPosition = new Vector3(j, -i, 0) - Center;
	                var block = childBlock.GetComponent<Block>();
	                block.Color = BlockColor;
	                blocks[i, j] = block;
                } 
            }
        }
    }
}
