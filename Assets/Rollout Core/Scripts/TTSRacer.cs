using UnityEngine;
using System.Collections.Generic;

/***********************
 * Racer.cs - Racer Prefab Builder and Motion Handler
 * 
 * This script has two primary roles, the first is to handle the connection (and instansiation [TODO])
 * of all the Objects embedded in the racer prefab. It also handles the motion of the racer, weather
 * it is player controlled or AI.
 * 
 * The main components of the prefab are the physics controller, which is an invisible rigidbody with
 * roughly the same size of the mesh. It is responsible for handling the simulation of physics on the 
 * character and provide realistic movement. It also is where the forces should be applied to to invoke
 * the characters movement. 
 * 
 * The body is the displayed mesh for the character, it simply needs to update its transform to match what
 * movment patterns we decide on, making movement much more realistic and predictable. It also needs to update 
 * It's position to match the rigidbodies.
 * 
 * The lights controlled contains the spotlights responsible for creating the light patterns on the ground.
 * It basically needs to update to match the transform of the body.
 * 
 * The current implementation relies on a vector called RealForward, it is the current desired direction of 
 * motion, the FloorNormal is the normal of the floor under the object. It should update and change the 'y'  
 * direction of the realforward vector.
 * 
 * The rest will be explained inline.
 * 
 * 
 * */

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TTSRacer : TTSBehaviour
{


	#region serialized settings
	public bool IsPlayerControlled = true;
	#endregion

	#region Internal Components
	public Transform displayMeshComponent;
	private AudioSource RacerSounds;
	private AudioSource RacerSfx;
	public AudioClip RollingSound;
	#endregion


	#region Internal Movement Operators
	private Vector3 IdleForwardVector;
	private Vector3 PreviousVelocity = new Vector3(1, 0, 0);
	public bool onGround = true;
	private float TiltRecoverySpeed = 0.1f;
	private float TiltAngle = 0.0f;
	public AudioClip[] DamageSounds;
	public GameObject SparksEmitter;
	public GameObject CurrentRig;
	public bool canMove = false;
	private float MinimumVelocityToAnimateSteering = 1.0f;

	private float resultAccel = 0.0f; // For sound calculation
	public float rpm = 0;
	#endregion

	#region gameplay vars
	public float TopSpeed = 250.0f;
	public float Acceleration = 8000.0f;
	public float Handling = 11000.0f;
	public enum PlayerType { Player, AI, Multiplayer };
	public PlayerType player = PlayerType.Player;
	#endregion

	private float smooth;
	private float stopSpeed = 0.05f;

	void Awake() {
		level.RegisterRacer(gameObject);
		//Get the body via tag.
		foreach (Transform child in transform) {
			if (child.gameObject.tag == "RacerDisplayMesh") {
				displayMeshComponent = child;
			}
		}

		if (!level.PerksEnabled) {
			this.GetComponent<TTSPerkManager>().enabled = false;
		}

		if (displayMeshComponent == null) {
			Debug.LogException(new UnassignedReferenceException());
		}
		IdleForwardVector = transform.forward;


		//Setup Audio Channel for Pitched Racer Sounds
		RacerSounds = gameObject.AddComponent<AudioSource>();
		RacerSounds.clip = RollingSound;
		RacerSounds.loop = true;
		RacerSounds.rolloffMode = AudioRolloffMode.Linear;
		RacerSounds.Play();

		//Setup Audio Channel for SFX
		RacerSfx = gameObject.AddComponent<AudioSource>();
		RacerSfx.rolloffMode = AudioRolloffMode.Linear;
		RacerSfx.volume = 0.5f;

		//Apply Attributes
		TopSpeed = TopSpeed + (100.0f * CurrentRig.GetComponent<TTSRig>().rigSpeed);
		Acceleration = Acceleration + (100.0f * CurrentRig.GetComponent<TTSRig>().rigAcceleration);
		Handling = Handling + (100.0f * CurrentRig.GetComponent<TTSRig>().rigHandling);
	}

	void FixedUpdate() {
		if (IsPlayerControlled && !level.raceHasFinished) {
			CalculateInputForces();
		}
		else {
			SlowToStop();
		}

		CalculateBodyOrientation();

		resultAccel = Mathf.Lerp(resultAccel, rigidbody.velocity.magnitude -PreviousVelocity.magnitude, 0.01f);
		PreviousVelocity = rigidbody.velocity;


	}

	void CalculateInputForces() {
		float vAmount = 0.0f, hAmount = 0.0f;

		if (player == PlayerType.Player) {
			vAmount = Input.GetAxis("Vertical");
			hAmount = Input.GetAxis("Horizontal");
		}

		// Vertical Input
		rpm = Mathf.Lerp(rpm, rpm + vAmount, 0.1f);
		rpm = Mathf.Clamp(rpm + ((vAmount == 0) ? -1 : 0), 0, 100);

		if (onGround && rigidbody.velocity.magnitude < TopSpeed && canMove) {
			rigidbody.AddForce(displayMeshComponent.forward * vAmount * Time.deltaTime * Acceleration);
		}
		else if (!onGround) {
			float gravity = -30;
			rigidbody.AddForce(displayMeshComponent.up * gravity);
		}

		// Horizontal Input
		if (canMove) {
			rigidbody.AddForce(displayMeshComponent.right * hAmount * Time.deltaTime * Acceleration);
		}
	}

	public void SlowToStop() {
		rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, new Vector3(0, 0, 0), stopSpeed);
	}

	void OnCollisionEnter(Collision collision) {

		onGround = true;
		if (collision.relativeVelocity.magnitude > 10) {
			vfx.DamageEffect(100.0f);
			//RacerSfx.volume = collision.relativeVelocity.magnitude / TopSpeed / 1.5f;
			RacerSfx.volume = collision.relativeVelocity.magnitude / 100.0f / 1.5f;
			RacerSfx.PlayOneShot(DamageSounds[Mathf.FloorToInt(Random.value * DamageSounds.Length)]);
		}

		//spawn sparks (TODO: move this to a component script)
		GameObject sparkClone = (GameObject)Instantiate(SparksEmitter);
		sparkClone.transform.position = collision.contacts[0].point;
		sparkClone.particleEmitter.emit = true;
		sparkClone.transform.forward = displayMeshComponent.forward;

	}

	void OnCollisionStay(Collision collision) {
		onGround = true;
	}

	void OnCollisionExit(Collision collision) {
		onGround = false;
	}

	void CalculateBodyOrientation() {

		//Facing Direction...
		if (new Vector2(rigidbody.velocity.x, rigidbody.velocity.z).magnitude > MinimumVelocityToAnimateSteering) {
			//based on rigidbody velocity.
			displayMeshComponent.forward = rigidbody.velocity;
			TiltAngle = Mathf.Lerp(TiltAngle, TTSUtils.GetRelativeAngle(rigidbody.velocity, PreviousVelocity) / 2, TiltRecoverySpeed);

			displayMeshComponent.RotateAround(displayMeshComponent.forward, TiltAngle);
			//set the idle vec, so it doesnt get janky.
			IdleForwardVector = displayMeshComponent.forward;
		}
		else {
			displayMeshComponent.forward = IdleForwardVector;
		}




	}

	void LateUpdate() {
		int offset = (resultAccel <=0) ? -10 : 15;

		//sound
		RacerSounds.pitch = Mathf.Max(Mathf.Lerp(RacerSounds.pitch, TTSUtils.Remap(rigidbody.velocity.magnitude + offset, 0f, 100.0f, 0.5f, 1.0f, false), 0.1f), 0);
		RacerSounds.volume = Mathf.Max(Mathf.Lerp(RacerSounds.volume, TTSUtils.Remap(rigidbody.velocity.magnitude + offset, 0f, 100.0f, 0.5f, 1f, false), 0.1f) * 1.5f, 0);
	}

	public float GetTiltAngle() {
		return TiltAngle;
	}

	public void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, IdleForwardVector * 10);

		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(transform.position, displayMeshComponent.forward * 10);

		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, rigidbody.velocity * 5);

		Gizmos.color = Color.cyan;
		Gizmos.DrawRay(transform.position, PreviousVelocity * 5);
	}
}


