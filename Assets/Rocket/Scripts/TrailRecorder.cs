using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRecorder : MonoBehaviour
{
	private List<Vector3> points = new List<Vector3>();
	private float time = 0;
	public float distanceThreshold = 0.5f;
	private LineRenderer lineRenderer;
	
    // Start is called before the first frame update
    void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.positionCount = 0;
	}
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnShowTrail += OnShowTrail;
	}
	
	void OnShowTrail(bool show) {
		if (show) {
			lineRenderer.enabled = true;
			if (lineRenderer.positionCount == 0 && points.Count > 0) {
				lineRenderer.positionCount = points.Count;
				Utility.ForEach(points, (i, pos) => {
					lineRenderer.SetPosition(i, pos + Vector3.back * 4);	
				});
			}
		}
		else {
			lineRenderer.enabled = false;
		}
	}
	
	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnShowTrail -= OnShowTrail;
	}

    // Update is called once per frame
    void Update()
    {
	    if (RocketGlobal.IsPaused || RocketGlobal.IsCompleted) {
	    	return;
	    }
	    time += Time.deltaTime;
	    if (points.Count == 0) {
	    	points.Add(transform.position);
	    } else if ((transform.position - points[points.Count-1]).magnitude >= distanceThreshold) {
	    	points.Add(transform.position);
	    }
    }
}
