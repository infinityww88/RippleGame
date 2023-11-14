using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Rocket/RingShatterProfile", fileName="RingShatterProfile", order=-1)]
public class RingShatterProfile : ScriptableObject
{
	public float innerRaidus = 5;
	public float outerRaidus = 7;
	public float minScale = 1f;
	public float maxScale = 3f;
	public List<GameObject> prefabs;
}
