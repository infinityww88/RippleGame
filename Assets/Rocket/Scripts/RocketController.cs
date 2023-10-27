using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using QFSW.QC;

public class RocketController : MonoBehaviour
{
	[SerializeField]
	private Transform leftEngine;
	
	[SerializeField]
	private Transform rightEngine;
	
	[SerializeField]
	private float force;
	
	private Rigidbody rigidbody;
	
	[SerializeField]
	private InputAction startLeftEngineAction;
	
	[SerializeField]
	private InputAction startRightEngineAction;
	
	[SerializeField]
	private InputAction reloadSceneAction;
	
	public Transform COM;
	
	private bool leftEngineStart = false;
	private bool rightEngineStart = false;
	
	public ParticleSystem leftParticle;
	public ParticleSystem rightParticle;
	
    // Start is called before the first frame update
    void Start()
	{
		ReadConfig();
		rigidbody = GetComponent<Rigidbody>();
		//rigidbody.centerOfMass = COM.localPosition;
		startLeftEngineAction.performed += ctx => OnStartLeftEngine();
		startRightEngineAction.performed += ctx => OnStartRightEngine();
	    
		startLeftEngineAction.canceled += ctx => OnStopLeftEngine();
		startRightEngineAction.canceled += ctx => OnStopRightEngine();
	    
		reloadSceneAction.performed += ctx => OnReloadScene();
	}
    
	private void ReadConfig() {
		force = PlayerPrefs.GetFloat("force", 100);
		Debug.Log("read config " + force);
	}
	
	[Command]
	private void SetForce(float force) {
		Debug.Log("force " + force);
		this.force = force;
		PlayerPrefs.SetFloat("force", force);
		PlayerPrefs.Save();
	}
    
	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		startLeftEngineAction.Enable();
		startRightEngineAction.Enable();
		reloadSceneAction.Enable();
		
		RocketGlobal.OnLeftOperateDown += OnStartLeftEngine;
		RocketGlobal.OnLeftOperateUp += OnStopLeftEngine;
		RocketGlobal.OnRightOperateDown += OnStartRightEngine;
		RocketGlobal.OnRightOperateUp += OnStopRightEngine;
		RocketGlobal.OnReloadScene += OnReloadScene;
	}
	
	// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
	protected void OnDrawGizmos()
	{
		if (rigidbody != null && rigidbody.IsSleeping()) {
			Gizmos.DrawSphere(transform.position, 1f);
		}
	}

	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		startLeftEngineAction.Disable();
		startRightEngineAction.Disable();
		reloadSceneAction.Disable();
		RocketGlobal.OnLeftOperateDown -= OnStartLeftEngine;
		RocketGlobal.OnLeftOperateUp -= OnStopLeftEngine;
		RocketGlobal.OnRightOperateDown -= OnStartRightEngine;
		RocketGlobal.OnRightOperateUp -= OnStopRightEngine;
		RocketGlobal.OnReloadScene -= OnReloadScene;
	}
	
	void OnStartLeftEngine() {
		SetEngineState(true, true);
	}
	
	void OnStopLeftEngine() {
		SetEngineState(true, false);
	}
	
	void OnStartRightEngine() {
		SetEngineState(false, true);
	}
	
	void OnStopRightEngine() {
		SetEngineState(false, false);
	}
	
	void SetEngineState(bool left, bool start) {
		if (left) {
			leftEngineStart = start;
			leftParticle.enableEmission = start;
			if (start) {
				//Debug.Log("left engine play");
				leftEngine.GetComponent<AudioSource>().Play();
			} else {
				//Debug.Log("left engine stop");
				leftEngine.GetComponent<AudioSource>().Stop();
			}
		}
		else {
			rightEngineStart = start;
			rightParticle.enableEmission = start;
			if (start) {
				//Debug.Log("right engine play");
				rightEngine.GetComponent<AudioSource>().Play();
			} else {
				//Debug.Log("right engine stop");
				rightEngine.GetComponent<AudioSource>().Stop();
			}
		}
	}

	void OnReloadScene() {
		SceneManager.LoadScene(0);
	}

    // Update is called once per frame
	void FixedUpdate()
    {
	    if (leftEngineStart) {
	    	rigidbody.AddForceAtPosition(transform.up * force, leftEngine.position);
	    }
	    if (rightEngineStart) {
	    	rigidbody.AddForceAtPosition(transform.up * force, rightEngine.position);
	    }
    }
    
	void Update() {
	}
}
