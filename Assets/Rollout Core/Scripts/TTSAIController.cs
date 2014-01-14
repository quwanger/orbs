using UnityEngine;
using System.Collections.Generic;


public class TTSAIController : ScriptableObject
{
	#region AI gameplay vars
	public int intelligence; // Can engage in certain maneuvers based on intelligence
	public bool debugMode = true; // Draws debug rays
	public float HARD_TURN_AMOUNT = 0.9f; // How hard the next turn will be to use the hard turn maneuver
	public int resolution = 4; // How many rays are cast to find way around obstacle. Higher = more accurate
	public int hardAngleInterval = 15; // Angle between rays cast to find way around in secondary path correction. Lower = more accurate
	public int foresight = 3; // How many future waypoints to check
	public float foresightDistance = 300; // Not implemented, but how close waypoints need to be to consider for foresight
	public float AISlowDownDistance = 300.0f; // How far ahead to consider slowing down for a turn
	public float hardTurnDistance = 100.0f; // When in hard turn maneuver, when to deploy actual turn
	public float turnCautiousness = 4.0f; // Affects how the racer will slow down at turns
	private Vector3 tempDestination; // Used in secondary path correction
	private float secondaryManeuverBuffer = 1.1f;
	private float maxSecondaryBuffer = 2.0f;
	private float secondaryBufferStep = 0.2f;
	#endregion

	#region AI persistent vars
	// public enum AIManeuverType {straight, hardTurn, blockedPath, secondaryBlockedPath};
	// AIManeuverType maneuver = AIManeuverType.straight;

	private float turnAmount = 0.0f;
	private Vector3 destination;
	private Vector3 detourDestination; // Used in the secondary blocked path

	private int counter = 0;
	#endregion


	public TTSAIController(int intelligence){
		this.intelligence = intelligence;

		if(intelligence > 1){
			hardTurnDistance = 75.0f;
		}

		randomizeValues();
	}

	// Randomize the distances and maybe the foresight to get more randomness
	public void randomizeValues(){ }

	/// Takes distance into account for how fast the racer must go.
	public float verticalInput(float prevInput, TTSWaypoint wp, Vector3 position, Vector3 velocity){
		// Get distance multiplier based on how far the racer is from the waypoint
		float distanceMultiplier = Mathf.Pow(Mathf.Min(1.0f, wp.getDistanceFrom(position) / AISlowDownDistance), turnCautiousness);

		if(intelligence > 2){ // Reverse if it's a hard turn
			if(turnAmount < HARD_TURN_AMOUNT &&	velocity.magnitude > (80.0f * turnAmount) && wp.getDistanceFrom(position) < ((1-turnAmount) * hardTurnDistance))
				return -1.0f;
		}

		float t = Mathf.Pow(turnAmount, 1-distanceMultiplier);

		return Mathf.Lerp(prevInput, t, 0.1f);
	}

	public Vector3 getDestination(Vector3 racerForward, TTSWaypoint next, Vector3 position) {
		turnAmount = turnCurveAmount(racerForward, next);

		// If there's a secondary destination that needs to be held for a bit
		if (counter > 0) {
			counter--;
			return destination;
		}

		// Straight Path
		if (turnAmount > HARD_TURN_AMOUNT) // Check to see if there needs to be a hard turn
			destination = next.getClosestPoint(position);
		// Hard turn
		else
			destination = hardTurnManeuver(next, position);

		// Blocked Path
		if(Physics.Linecast(position, destination, TTSUtils.LayerMask(10))){
			destination = blockedPathManeuver(next, position);
		}

		// Secondary Blocked Path
		RaycastHit hit;
		if(Physics.Linecast(position, destination, out hit, TTSUtils.LayerMask(10))){
			destination = secondaryBlockedPathManeuver(next, position, hit);
		}

		return destination;
	}

	// Used when the upcoming turns are very drastic
	public Vector3 hardTurnManeuver(TTSWaypoint nextWP, Vector3 position) {
		Vector3 destination = new Vector3();

		Vector3 point = Vector3.Project(waypointForwardForesight(foresight, nextWP).normalized, nextWP.colliderLine);

		Debug.DrawRay(nextWP.position, point * nextWP.boxWidth / 2);

		if (nextWP.getDistanceFrom(position) > hardTurnDistance)
			destination = nextWP.position - (point * nextWP.boxWidth / 2);
		else {
			destination = nextWP.position + (point.normalized * nextWP.boxWidth / 2);
		}

		return destination;
	}

	// Find ways around any obstacles
	public Vector3 blockedPathManeuver(TTSWaypoint wp, Vector3 position) {
		int resolution = 5;
		Vector3 destination = wp.position;
		destination.y = position.y;
		Vector3 pnt = new Vector3();

		destination = wp.position;

		// So that we make as many checks as resolutions;
		for (float i = 0; i < resolution; i++) { // Start from right to left.
			pnt = wp.getPointOn(i / (resolution-1));
			pnt.y = position.y;
			
			if(debugMode)
				Debug.DrawLine(position, pnt);

			if (!Physics.Linecast(position, pnt, TTSUtils.LayerMask(10)) && Vector3.Distance(position, pnt) < Vector3.Distance(position, destination)) {
				destination = pnt;
			}
		}

		return destination;
	}

	public Vector3 secondaryBlockedPathManeuver(TTSWaypoint next, Vector3 position, RaycastHit hit) {
		return secondaryBlockedPathManeuver(next, position, hit, secondaryManeuverBuffer);
	}

	public Vector3 secondaryBlockedPathManeuver(TTSWaypoint next, Vector3 position, RaycastHit hit, float multiplier){

		if (detourDestination != Vector3.zero) {
			Vector3 tempPos = next.getClosestSeenPoint(detourDestination, resolution);
			if (!Physics.Linecast(position, detourDestination, TTSUtils.LayerMask(10)) && !Physics.Linecast(detourDestination, tempPos, TTSUtils.LayerMask(10))) {
				Debug.Log("Detour Destination reused.");
				return detourDestination;
			}
			else{
				detourDestination = Vector3.zero;
			}
		}

		float distance = (hit.point - position).magnitude *  multiplier;

		Vector3 rotatedVec1, rotatedVec2;
		for(int i=1; i<=45.0f/hardAngleInterval; i++){
			float checkAngle = hardAngleInterval * i;

			// Get the two paths
			rotatedVec1 = TTSUtils.RotateAround(hit.point, position, new Vector3(0, checkAngle, 0));
			rotatedVec2 = TTSUtils.RotateAround(hit.point, position, new Vector3(0, -checkAngle, 0));

			Vector3 nextPosition1 = next.getClosestSeenPoint(rotatedVec1, resolution);
			Vector3 nextPosition2 = next.getClosestSeenPoint(rotatedVec2, resolution);

			Debug.DrawLine(position, rotatedVec1);
			Debug.DrawLine(position, rotatedVec2);
			Debug.DrawLine(rotatedVec1, nextPosition1);
			Debug.DrawLine(rotatedVec2, nextPosition2);

			float dist1 = -1.0f, dist2 = -1.0f;

			// Check to see if there's anything in the way
			if (!Physics.Linecast(position, rotatedVec1, TTSUtils.LayerMask(10)) && !Physics.Linecast(nextPosition1, rotatedVec1, TTSUtils.LayerMask(10))) {
				dist1 = (next.position - rotatedVec1).magnitude;
			}
			if (!Physics.Linecast(position, rotatedVec2, TTSUtils.LayerMask(10)) && !Physics.Linecast(nextPosition2, rotatedVec2, TTSUtils.LayerMask(10))) {
				dist2 = (next.position - rotatedVec2).magnitude;
			}

			if (dist2 < 0.0f || dist1 < dist2) {
				destination = detourDestination = rotatedVec1;
			}
			else if (dist1 < 0.0f || dist2 < dist1) {
				destination = detourDestination = rotatedVec2;
			}
			else if(multiplier < maxSecondaryBuffer){
				return secondaryBlockedPathManeuver(next, position, hit, multiplier + secondaryBufferStep);
			}
			else { // NOthing worked. Abandon all hope.
				return next.position;
			}
		}

		return destination;
	}

	/// How similar the racer's forward and the waypoints forward are (0.0f -> 1.0f = hard -> no turn )
	public float turnCurveAmount(Vector3 racerForward, TTSWaypoint wp) {
		// Compare racer forward with waypoint forwards
		Vector3 tempForward = waypointForwardForesight(foresight, wp).normalized;
		float speed = Vector3.Project(tempForward, racerForward).magnitude;

		return Mathf.Sqrt(speed);
	}

	public TTSWaypoint getClosestWaypoint(List<TTSWaypoint> wp, Vector3 from){
		TTSWaypoint closest = null;

		foreach (TTSWaypoint waypoint in wp) {
			if (closest == null) {
				closest = waypoint;
			}
			else {
				if(waypoint.getDistanceFrom(from) < closest.getDistanceFrom(from))
					closest = waypoint;
			}
		}

		return closest;
	}

	private Vector3 waypointForwardForesight(int foresight, TTSWaypoint wp) {
		Vector3 forward = wp.forwardLine * foresight;

		if(foresight != 1){
			foresight--;

			foreach (TTSWaypoint next in wp.nextWaypoints) {
				forward += waypointForwardForesight(foresight, next);
			}
		}

		return forward;
	}
}