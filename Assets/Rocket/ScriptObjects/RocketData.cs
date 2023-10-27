using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketData : ScriptableObject
{
	public float maxLife = 100;
	public float maxFuel = 100;
	public float damagePerSpeed = 1f;
	public float fuelPerSec = 1f;
}
