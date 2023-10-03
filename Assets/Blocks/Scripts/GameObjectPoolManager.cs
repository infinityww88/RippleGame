using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class GameObjectPoolManager : SerializedMonoBehaviour
{
	[SerializeField]
	private Dictionary<string, GameObject> poolConfig;
	
	private Dictionary<string, ObjectPool<GameObject>> pools;
	
    // Start is called before the first frame update
    void Start()
	{
		pools = new	Dictionary<string, ObjectPool<GameObject>>();
		poolConfig.ForEach(kv => {
			Debug.Log($"pool add {kv.Key}");
	    	pools.Add(kv.Key, new ObjectPool<GameObject>(CreatePooledItemFunc(kv.Key),
	    		OnTakeFromPool, OnReturnToPool, OnDestroyPooledGameObject));
	    });
    }
    
	void Assert(string pool) {
		if (!pools.ContainsKey(pool)) {
			throw new KeyNotFoundException($"GameObject pool key {pool} not found");
		}
	}
    
	public GameObject Get(string pool) {
		Assert(pool);
		return pools[pool].Get();
	}
	
	public void Release(string pool, GameObject go) {
		Assert(pool);
		pools[pool].Release(go);
	}
	
	public void Clear(string pool) {
		Assert(pool);
		pools[pool].Clear();
	}
    
	Func<GameObject> CreatePooledItemFunc(string key) {
		return () => {
			Debug.Log($"Pool {key} Create");
			return Instantiate(poolConfig[key]);
		};
	}
	
	void OnReturnToPool(GameObject go) {
		Debug.Log($"Pool return {go}");
		go.SetActive(false);
	}
	
	void OnTakeFromPool(GameObject go) {
		Debug.Log($"Pool take {go}");
		go.SetActive(true);
	}
	
	void OnDestroyPooledGameObject(GameObject go) {
		Debug.Log($"Pool destroy {go}");
		Destroy(go);
	}
	
	[Button]
	void Test() {
		string pool = "EmptyBlock";
		GameObject o0 = Get(pool);
		GameObject o1 = Get(pool);
		Release(pool, o0);
		Release(pool, o1);
		
		GameObject o2 = Get(pool);
	}
}
