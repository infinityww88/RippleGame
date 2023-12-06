using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using DG.Tweening;
using Com.LuisPedroFonseca.ProCamera2D;
using ScriptableObjectArchitecture;

public class RocketController : MonoBehaviour
{
	private enum State {
		Running,
		Landed,
		Destroyed
	}
	
	private State state = State.Running;

	[SerializeField]
	private Transform leftEngine;
	
	[SerializeField]
	private Transform rightEngine;
	
	[SerializeField]
	private FloatVariable force;
	
	private Rigidbody2D rigidbody;
	
	private AudioSource audioSouce;
	
	public Transform COM;
	
	private bool leftEngineStart = false;
	private bool rightEngineStart = false;
	
	public bool LeftEngineRunning => leftEngineStart;
	public bool RightEngineRunning => rightEngineStart;
	
	public ParticleSystem leftParticle;
	public ParticleSystem rightParticle;
	
	public AudioClip rocketImpactClip;
	public AudioClip engineImpactClip;
	
	public bool soundOn = true;
	
	// Awake is called when the script instance is being loaded.
	protected void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
	}
	
    // Start is called before the first frame update
    void Start()
	{
		soundOn = ES3.Load<bool>("sound_on", true);
		
		audioSouce = GetComponent<AudioSource>();
		var proCamera = Camera.main.GetComponent<ProCamera2D>();
		proCamera.AddCameraTarget(transform);
		
		CameraController.Instance.rocket = transform;
	}

	// This function is called when the object becomes enabled and active.
	protected void OnEnable()
	{
		RocketGlobal.OnLandingSuccess += OnLandingSuccess;
		RocketGlobal.OnPause += OnPause;
		RocketGlobal.OnResume += OnResume;
		
		RocketGlobal.OnLeftOperateDown += OnStartLeftEngine;
		RocketGlobal.OnRightOperateDown += OnStartRightEngine;
		RocketGlobal.OnLeftOperateUp += OnStopLeftEngine;
		RocketGlobal.OnRightOperateUp += OnStopRightEngine;
	}
	
	public void StartTutorial() {
		rigidbody.bodyType = RigidbodyType2D.Kinematic;
	}
	public void StopTutorial() {
		rigidbody.bodyType = RigidbodyType2D.Dynamic;
	}
	
	void OnPause() {
		rigidbody.simulated = false;
	}
	
	void OnResume() {
		rigidbody.simulated = true;
	}
	
	// This function is called when the MonoBehaviour will be destroyed.
	protected void OnDestroy()
	{
		
	}
	
	void OnLandingSuccess() {
		SetEngineState(true, false);
		SetEngineState(false, false);
		state = State.Landed;
	}
	
	// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
	protected void OnDrawGizmos()
	{
	}

	// This function is called when the behaviour becomes disabled () or inactive.
	protected void OnDisable()
	{
		RocketGlobal.OnLandingSuccess -= OnLandingSuccess;
		RocketGlobal.OnPause -= OnPause;
		RocketGlobal.OnResume -= OnResume;
		
		RocketGlobal.OnLeftOperateDown -= OnStartLeftEngine;
		RocketGlobal.OnRightOperateDown -= OnStartRightEngine;
		RocketGlobal.OnLeftOperateUp -= OnStopLeftEngine;
		RocketGlobal.OnRightOperateUp -= OnStopRightEngine;
	}
	
	void OnStartLeftEngine() {
		//RocketGlobal.OnLeftOperateDown?.Invoke();
		SetEngineState(true, true);
	}
	
	void OnStopLeftEngine() {
		//RocketGlobal.OnLeftOperateUp?.Invoke();
		SetEngineState(true, false);
	}
	
	void OnStartRightEngine() {
		//RocketGlobal.OnRightOperateDown?.Invoke();
		SetEngineState(false, true);
	}
	
	void OnStopRightEngine() {
		//RocketGlobal.OnRightOperateUp?.Invoke();
		SetEngineState(false, false);
	}
	
	void SetEngineState(bool left, bool start) {
		if (RocketGlobal.IsPaused || state != State.Running) {
			return;
		}
		
		if (left) {
			leftEngineStart = start;
			leftParticle.enableEmission = start;
			if (start) {
				//Debug.Log("left engine play");
				if (soundOn) {
					leftEngine.GetComponent<AudioSource>().Play();
				}
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
				if (soundOn) {
					rightEngine.GetComponent<AudioSource>().Play();
				}
			} else {
				//Debug.Log("right engine stop");
				rightEngine.GetComponent<AudioSource>().Stop();
			}
		}
	}
	
	// Sent when an incoming collider makes contact with this object's collider (2D physics only).
	protected void OnCollisionEnter2D(Collision2D collisionInfo)
	{	
		if (collisionInfo.gameObject.tag == "TargetPlatform" && collisionInfo.otherCollider.tag == "RocketEngine") {
			Debug.Log("OK");
			if (state == State.Running) {
				//audioSouce.PlayOneShot(engineImpactClip);
			} else if (state == State.Destroyed) {
				var vel = collisionInfo.relativeVelocity;
				vel = Quaternion.Euler(0, 0, 45) * vel;
				Debug.Log($"Destroyed engine impact {vel.normalized} {transform.InverseTransformPoint(collisionInfo.contacts[0].point)}");
				rigidbody.AddForceAtPosition(-vel.normalized * 500, collisionInfo.contacts[0].point);
			}
		} else {
			if (state == State.Running) {
				RocketGlobal.OnRocketHit();
				SetEngineState(true, false);
				SetEngineState(false, false);
				state = State.Destroyed;
				if (soundOn) {
					audioSouce.PlayOneShot(rocketImpactClip);
				}
				Debug.Log("Emit landing fail");
				RocketGlobal.OnLandingFail();
				GetComponent<RocketLandingChecker>().enabled = false;
			}
		}
	}

    // Update is called once per frame
	void FixedUpdate()
	{
		if (state != State.Running) {
			if (transform.position.magnitude > 100f) {
				gameObject.SetActive(false);
			}
			return;
		}
		
	    if (leftEngineStart) {
	    	rigidbody.AddForceAtPosition(transform.up * force, leftEngine.position);
	    }
	    if (rightEngineStart) {
	    	rigidbody.AddForceAtPosition(transform.up * force, rightEngine.position);
	    }
    }
}
