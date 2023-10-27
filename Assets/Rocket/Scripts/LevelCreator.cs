using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using Sirenix.OdinInspector;
using System.Linq;

public class LevelCreator : MonoBehaviour
{
	public GameObjectCollection rockPrefabs;
	public float radius = 10;
	public Color rectColor;
	public int rockNum = 10;
	public Vector2 scaleRange = Vector2.one;
	private List<Bounds> allBounds;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
	[Button(ButtonSizes.Large), GUIColor(0, 1, 0, 1)]
	void Generate() {
		Transform c = null;
		
		if (transform.childCount > 0) {
			c = transform.GetChild(0);
			c.SetParent(null);
			DestroyImmediate(c.gameObject);
			allBounds.Clear();
		}
		
		c = new GameObject("level").transform;
		c.SetParent(transform, false);
		for (int i = 0; i < rockNum; i++) {
			var t = Random.Range(0, rockPrefabs.Count);
			GameObject o = rockPrefabs[t];
			var rock = Instantiate(o, Vector3.zero, Quaternion.identity, c);
			bool set = false;
			for (int k = 0; k < 100; k++) {
				Vector3 center = Random.insideUnitCircle * radius;
				var scale = Random.Range(scaleRange.x, scaleRange.y);
				rock.transform.localPosition = center;
				rock.transform.localScale = Vector3.one * scale;
				Bounds bounds = rock.GetComponent<MeshCollider>().bounds;
				bounds.center = rock.transform.position;
				bounds.size *= scale;
				if (allBounds.All(b => !b.Intersects(bounds))) {
					set = true;
					allBounds.Add(bounds);
					break;
				}
			}
			if (!set) {
				break;
			}
		}
	}
    
	// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
	protected void OnDrawGizmos()
	{
		Gizmos.color = rectColor;
		/*
		Gizmos.DrawLine(transform.position + new Vector3(-width, height) /2,
			transform.position + new Vector3(width, height) / 2);
		Gizmos.DrawLine(transform.position + new Vector3(-width, -height) /2,
			transform.position + new Vector3(width, -height) / 2);
		Gizmos.DrawLine(transform.position + new Vector3(-width, height) /2,
			transform.position + new Vector3(-width, -height) / 2);
		Gizmos.DrawLine(transform.position + new Vector3(width, height) /2,
		transform.position + new Vector3(width, -height) / 2);
		*/
		
		Gizmos.DrawWireSphere(transform.position, radius);
			
		if (allBounds != null) {
			allBounds.ForEach(b => {
				Gizmos.DrawWireCube(b.center, b.size);
			});
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
