using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelLogic : MonoBehaviour
{
	public PatternDataCollection patternDataList;
	public GameObject patternPrefab;
    public Color32Collection colorList;
    private const int NUM = 3;
    private HashSet<GameObject> remainPatterns = new HashSet<GameObject>();
    public BlockGrid grid;
    public RectTransform panel;

    private void Start()
	{
		InitPattern();
        NewPatterns();
    }

	private void InitPattern() {
		
	}

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    private void NewPatterns()
    {
        for (int i = 0; i < NUM; i++)
        {
	        int index = Random.Range(0, patternDataList.Count - 1);
	        GameObject o = Instantiate(patternPrefab, transform.GetChild(i));
	        var pattern = o.GetComponent<BlockPattern>();
	        pattern.GeneratePattern(patternDataList[index]);
            o.transform.localPosition = Vector3.zero;
            remainPatterns.Add(o);
            index = Random.Range(0, colorList.Count - 1);
            Debug.Log("random color " + index + ", " + colorList.Count);
	        pattern.BlockColor = colorList[index];
	        pattern.SetSmall();
        }
    }
    public void CancelPattern(GameObject go)
    {
        go.transform.localPosition = Vector3.zero;
    }

    public void ApplyPattern(GameObject go)
    {
        remainPatterns.Remove(go);

        if (remainPatterns.Count == 0)
        {
            NewPatterns();
        }

        bool hasAvailPattern = false;
        foreach (GameObject o in remainPatterns)
        {
            BlockPattern bp = o.GetComponent<BlockPattern>();
            if (grid.AvailableForPattern(bp))
            {
                hasAvailPattern = true;
                break;
            }
        }
        if (!hasAvailPattern)
        {
            LevelOver();
        }
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(0);
    }

    void LevelOver()
    {
        Debug.Log("LevelOver");
        panel.gameObject.SetActive(true);
        remainPatterns.ForEach(go =>
        {
        	
            Destroy(go.GetComponent<EventTrigger>());
        });
    }
}
