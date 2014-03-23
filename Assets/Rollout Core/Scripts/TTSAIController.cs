using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class TTSAIController : TTSBehaviour
{
	#region AI gameplay vars
	public int intelligence = 5; // Can engage in certain maneuvers based on intelligence
	public bool debugMode = false; // Draws debug rays
	public float HARD_TURN_AMOUNT = 0.9f; // How hard the next turn will be to use the hard turn maneuver
	public int resolution = 4; // How many rays are cast to find way around obstacle. Higher = more accurate
	public int hardAngleInterval = 10; // Angle between rays cast to find way around in secondary path correction. Lower = more accurate
	public int foresight = 3; // How many future waypoints to check
	public float foresightDistance = 300; // Not implemented, but how close waypoints need to be to consider for foresight
	public float AISlowDownDistance = 300.0f; // How far ahead to consider slowing down for a turn
	public float hardTurnDistance = 100.0f; // When in hard turn maneuver, when to deploy actual turn
	public float turnCautiousness = 4.0f; // Affects how the racer will slow down at turns
	private Vector3 tempDestination; // Used in secondary path correction
	private float secondaryManeuverBuffer = 1.0f;
	private float maxSecondaryBuffer = 10.0f;
	private float secondaryBufferStep = 2.0f;

	private float racerBufferDistance = 20.0f; // How close a racer has to be to affect it.
	private float racerAvoidStrength = 5.0f;

	public float wallAvoidanceDistance = 2.0f; // How close the wall could be until it's avoided
	public float wallAvoidanceStrength = 2.0f;
	#endregion

	#region AI persistent vars
	// public enum AIManeuverType {straight, hardTurn, blockedPath, secondaryBlockedPath};
	// AIManeuverType maneuver = AIManeuverType.straight;

	private float turnAmount = 0.0f;
	private Vector3 destination;
	private Vector3 detourDestination; // Used in the secondary blocked path
	private Vector3 velocity;

	private int counter = 0;
	#endregion


	public void Awake() {
		if (intelligence > 1) {
			hardTurnDistance = 75.0f;
		}

		randomizeValues();
	}

	// Randomize the distances and maybe the foresight to get more randomness
	public void randomizeValues() {

	}

	/// Takes distance into account for how fast the racer must go.
	public float verticalInput(float prevInput, TTSWaypoint wp, Vector3 position, Vector3 racerVelocity) {
		velocity = racerVelocity;
		// Get distance multiplier based on how far the racer is from the waypoint
		float distanceMultiplier = Mathf.Pow(Mathf.Min(1.0f, wp.getDistanceFrom(position) / AISlowDownDistance), turnCautiousness);

		if (intelligence > 2) { // Reverse if it's a hard turn
			if (turnAmount < HARD_TURN_AMOUNT && racerVelocity.magnitude > (80.0f * turnAmount) && wp.getDistanceFrom(position) < ((1 - turnAmount) * hardTurnDistance))
				return -1.0f;
		}

		float t = Mathf.Pow(turnAmount, 1 - distanceMultiplier);

		return Mathf.Lerp(prevInput, t, 0.1f);
	}

	public Vector3 getDestination(Transform racerTransform, TTSWaypoint next, Vector3 position) {
		Vector3 racerForward = racerTransform.forward;
		turnAmount = turnCurveAmount(racerForward, next);

		// If there's a secondary destination that needs to be held for a bit
		if (counter > 0) {
			counter--;
			return destination;
		}

		// Straight Path
		if (turnAmount > HARD_TURN_AMOUNT) { // Check to see if there needs to be a hard turn
			if (debugMode) Debug.Log("Straight Path");
			destination = next.getClosestPoint(position);
			destination = Vector3.Lerp(next.position, destination, 0.9f);
		}
		// Hard turn
		else {
			if (debugMode) Debug.Log("Hard Turn");
			destination = hardTurnManeuver(next, position);
		}

		// Blocked Path
		if (Physics.Linecast(position, destination, TTSUtils.ExceptLayerMask(10))) {
			if (debugMode) Debug.Log("Blocked Path");

			destination = blockedPathManeuver(next, position);
		}

		// Secondary Blocked Path
		RaycastHit hit;
		if (Physics.Linecast(position, destination, out hit, TTSUtils.ExceptLayerMask(10))) {
			if (debugMode) Debug.Log("Secondary Blocked Path");

			destination = secondaryBlockedPathManeuver(next, position, hit);
		}

		destination += racerRelative(position, racerForward) * Mathf.Min(1.0f, next.getDistanceFrom(position) / racerBufferDistance);

		destination += wallAvoidance(position, racerTransform);

		return destination;
	}

	private Vector3 wallAvoidance(Vector3 position, Transform transform) {
		Vector3 direction = Vector3.zero;
		Vector3 origin = position + (transform.forward * 1.0f) + Vector3.up * 0.1f;
		Vector3 wing = TTSUtils.FlattenVector(transform.right).normalized * wallAvoidanceDistance;

			Debug.DrawRay(origin, wing);
			Debug.DrawRay(origin, -wing);

		if (Physics.Linecast(origin, origin + wing, TTSUtils.ExceptLayerMask(10))) {
			direction += -wing * wallAvoidanceStrength;
		}
		if (Physics.Linecast(origin, origin - wing, TTSUtils.ExceptLayerMask(10))) {
			direction += wing * wallAvoidanceStrength;
		}

		if (direction == Vector3.zero) {
			origin -= Vector3.up * 1.0f;
			wing = TTSUtils.FlattenVector(transform.right).normalized * 1.25f;

			Debug.DrawRay(origin, wing);
			Debug.DrawRay(origin, -wing);

			if (Physics.Linecast(origin, origin + wing, TTSUtils.ExceptLayerMask(10))) {
				direction += -wing * wallAvoidanceStrength;
			}
			if (Physics.Linecast(origin, origin - wing, TTSUtils.ExceptLayerMask(10))) {
				direction += wing * wallAvoidanceStrength;
			}
		}

		return direction;
	}

	private Vector3 racerRelative(Vector3 position, Vector3 racerForward) {
		Vector3 direction = Vector3.zero;

		foreach (GameObject racer in racers) {
			if (racer.Equals(gameObject))
				continue;

			Vector3 dir = racer.transform.position - position;

			Vector3 normalizedDir = dir.normalized;
			float directionalStrength = Mathf.Pow(1 - Vector3.Project(normalizedDir, racerForward).magnitude, 3); // Stronger avoidance when a racer is on the side than front.
			float speedStrength = Mathf.Clamp01(TTSUtils.FlattenVector(velocity).magnitude / 100.0f);

			if (dir.magnitude < racerBufferDistance) {
				//Debug.DrawLine(position, racer.transform.position);
				direction -= dir.normalized * Mathf.Pow(50.0f / dir.magnitude, 2) * directionalStrength * speedStrength;
			}
		}

		return direction * racerAvoidStrength;
	}

	// Used when the upcoming turns are very drastic
	public Vector3 hardTurnManeuver(TTSWaypoint nextWP, Vector3 position) {
		Vector3 destination = new Vector3();

		Vector3 point = Vector3.Project(waypointForwardForesight(foresight, nextWP).normalized, nextWP.colliderLine);

		if (debugMode)
			Debug.DrawRay(nextWP.position, point * nextWP.boxWidth / 2);

		if (nextWP.getDistanceFrom(position) > hardTurnDistance)
			destination = nextWP.position - (point * nextWP.boxWidth / 2);
		else {
			destination = nextWP.position + (point.normalized * nextWP.boxWidth / 2);
		}

		destination.y = Mathf.Clamp(position.y, nextWP.position.y - nextWP.boxHeight / 2, nextWP.position.y + nextWP.boxHeight / 2);

		return destination;
	}

	// Find ways around any obstacles
	public Vector3 blockedPathManeuver(TTSWaypoint wp, Vector3 position) {
		int resolution = 5;
		Vector3 destination = wp.position;
		destination.y = position.y;
		Vector3 pnt = new Vector3();

		// So that we make as many checks as resolutions;
		for (float i = 0; i < resolution; i++) { // Start from right to left.
			pnt = wp.getPointOn(i / (resolution - 1));
			pnt.y = position.y;

			if (!Physics.Linecast(position, pnt, TTSUtils.ExceptLayerMask(10)) && Vector3.Distance(position, pnt) < Vector3.Distance(position, destination)) {
				if (debugMode)
					Debug.DrawLine(position, pnt);

				destination = pnt;
			}
		}

		return destination;
	}

	public Vector3 secondaryBlockedPathManeuver(TTSWaypoint next, Vector3 position, RaycastHit hit) {
		return secondaryBlockedPathManeuver(next, position, hit, secondaryManeuverBuffer);
	}

	public Vector3 secondaryBlockedPathManeuver(TTSWaypoint next, Vector3 position, RaycastHit hit, float multiplier) {

		if (detourDestination != Vector3.zero) {
			Vector3 tempPos = next.getClosestSeenPoint(detourDestination, resolution);
			if (!Physics.Linecast(position, detourDestination, TTSUtils.ExceptLayerMask(10)) && !Physics.Linecast(detourDestination, tempPos, TTSUtils.ExceptLayerMask(10))) {
				// Debug.Log("Detour Destination reused.");
				return detourDestination;
			}
			else {
				detourDestination = Vector3.zero;
			}
		}

		float distance = (hit.point - position).magnitude * multiplier;

		Vector3 rotatedVec1, rotatedVec2;
		for (int i = 1; i <= 90.0f / hardAngleInterval; i++) {
			float checkAngle = hardAngleInterval * i;

			// Get the two paths
			rotatedVec1 = TTSUtils.RotateScaleAround(hit.point, position, new Vector3(0, checkAngle, 0), multiplier);
			rotatedVec2 = TTSUtils.RotateScaleAround(hit.point, position, new Vector3(0, -checkAngle, 0), multiplier);

			rotatedVec1.y = rotatedVec2.y = position.y;

			Vector3 nextPosition1 = next.getClosestPoint(rotatedVec1);
			Vector3 nextPosition2 = next.getClosestPoint(rotatedVec2);

			float dist1 = -1.0f, dist2 = -1.0f;

			if (debugMode) {
				Debug.DrawLine(position, rotatedVec1);
				Debug.DrawLine(rotatedVec1, nextPosition1);
				Debug.DrawLine(position, rotatedVec2);
				Debug.DrawLine(rotatedVec2, nextPosition2);
			}

			// Check to see if there's anything in the way
			if (!Physics.Linecast(position, rotatedVec1, TTSUtils.ExceptLayerMask(10)) && !Physics.Linecast(nextPosition1, rotatedVec1, TTSUtils.ExceptLayerMask(10))) {
				dist1 = (next.position - rotatedVec1).magnitude;
			}
			if (!Physics.Linecast(position, rotatedVec2, TTSUtils.ExceptLayerMask(10)) && !Physics.Linecast(nextPosition2, rotatedVec2, TTSUtils.ExceptLayerMask(10))) {
				dist2 = (next.position - rotatedVec2).magnitude;
			}

			// Return right destination
			if (dist1 == -1.0f && dist2 == -1.0f) {
				continue;
			}
			else if ((dist2 == -1.0f && dist1 != -1.0f) || (dist1 < dist2 && dist1 != -1.0f)) {
				return destination = detourDestination = rotatedVec1;
			}
			else if ((dist1 == -1.0f && dist2 != -1.0f) || (dist2 < dist1 && dist2 != -1.0f)) {
				return destination = detourDestination = rotatedVec2;
			}
		}

		// Retry with new multiplier if it didn't work.
		if (multiplier < maxSecondaryBuffer) {
			destination = detourDestination = secondaryBlockedPathManeuver(next, position, hit, multiplier + secondaryBufferStep);
		}
		else { // NOthing worked. Abandon all hope.
			if (debugMode) { Debug.Log("No way out."); }

			destination = next.position;
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

	public TTSWaypoint getClosestWaypoint(List<TTSWaypoint> wp, Vector3 from) {
		TTSWaypoint closest = null;

		foreach (TTSWaypoint waypoint in wp) {
			if (closest == null) {
				closest = waypoint;
			}
			else {
				if (waypoint.getDistanceFrom(from) < closest.getDistanceFrom(from))
					closest = waypoint;
			}
		}

		return closest;
	}

	private Vector3 waypointForwardForesight(int foresight, TTSWaypoint wp) {
		Vector3 forward = wp.forwardLine * foresight;

		if (foresight != 1) {
			foresight--;

			foreach (TTSWaypoint next in wp.nextWaypoints) {
				forward += waypointForwardForesight(foresight, next);
			}
		}

		return forward;
	}

	public void OnDrawGizmos() {
		//Gizmos.color = Color.green;
		//Gizmos.DrawWireSphere(transform.position, racerBufferDistance);
	}
}