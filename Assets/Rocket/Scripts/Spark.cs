using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Spark : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem particleSystem;

	[Button(ButtonSizes.Medium)]
	public void Emit() {
		particleSystem.Emit(1);
	}
}
