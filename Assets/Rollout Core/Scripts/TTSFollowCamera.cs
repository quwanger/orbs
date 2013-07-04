using UnityEngine;
using System.Collections.Generic;

public class TTSFollowCamera : MonoBehaviour
{

    public enum cameraModes { THIRD_PERSON, FIRST_PERSON };

    public cameraModes CameraMode;

    // The target we are following
    public Transform target;

    // The distance in the x-z plane to the target
    public float distance = 5.0f;

    // the height we want the camera to be above the target
    private float height = 4.0f;
    // How much we 
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;

    private float racerTopSpeed = -1.0f;

    // Camera tilt
    private float tiltAngle = 0.0f;

    // Shakiness factor
    public float shakiness = 0.15f;

    // Camera Effects handles
    public float baseDistance = 4.0f;
    public float maxDistance = 6.0f;

    public float baseHeight = 4.0f;
    public float minHeight = 0.5f;

    public float Base_FOV = 72.9f;
    public float Max_FOV = 77.0f;

    // Start effect percentages
    public float distanceStartEffect = 0.0f;
    public float heightStartEffect = 0.6f;
    public float fovStartEffect = 0.6f;
    public float shakeStartEffect = 0.7f;

    void Start() {
        // Move this to ThirdPeron(), and turn off clamps if boost will change the top speed.
        racerTopSpeed = target.parent.GetComponent<TTSRacer>().TopSpeed;
    }

    void FixedUpdate() {
        if (CameraMode == cameraModes.THIRD_PERSON) {
            ThirdPerson();
        }
        if (CameraMode == cameraModes.FIRST_PERSON) {
            this.transform.position = target.transform.position + transform.forward * 2;
            this.transform.rotation = target.transform.rotation;

            if (target.parent.GetComponent<TTSRacer>().rigidbody.velocity.sqrMagnitude > 3000) {
                transform.position += Random.insideUnitSphere * 0.06f;
            }

            if (GetComponent<Vignetting>().chromaticAberration > 0.0f) {
                GetComponent<Vignetting>().chromaticAberration = Mathf.Lerp(GetComponent<Vignetting>().chromaticAberration, 0.0f, 0.07f);
            }
        }
    }

    void ThirdPerson() {
        // Early out if we don't have a target
        if (!target)
            return;

        // Speed squared magnitude
        float speed = target.parent.GetComponent<TTSRacer>().rigidbody.velocity.magnitude;

        // Calculate the current rotation angles
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // Change the height that the camera should be above the ball
        height = Mathf.Lerp(height, baseHeight - TTSUtils.Remap(speed, racerTopSpeed * heightStartEffect, racerTopSpeed, 0, baseHeight - minHeight, true), 0.03f);

        float wantedRotationAngle = target.eulerAngles.y;
        float wantedHeight = target.position.y + height;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        transform.position = target.position;

        transform.position -= currentRotation * Vector3.forward * distance;


        // Always look at the target
        transform.LookAt(target);

        if (Input.GetKey(KeyCode.LeftShift)) {
            transform.RotateAround(Vector3.up, 180);
        }

        // Set the height of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        transform.position -= currentRotation * Vector3.forward * distance;

        // Camera Distance according to speed
        distance = Mathf.Lerp(distance, TTSUtils.Remap(speed, racerTopSpeed * distanceStartEffect, racerTopSpeed, baseDistance, maxDistance, true), 0.02f);

        // Camera FOV according to speed
        camera.fov = Mathf.Lerp(camera.fov, TTSUtils.Remap(speed, 40, racerTopSpeed*fovStartEffect, Base_FOV, Max_FOV, true), 0.05f);

        // Camera Shake. (Note: If statement can be removed)
        if (speed > racerTopSpeed * shakeStartEffect) {

            transform.position += Random.insideUnitSphere * TTSUtils.Remap(speed, racerTopSpeed * shakeStartEffect, racerTopSpeed, 0, shakiness, true);

            transform.LookAt(target);

            transform.position += Random.insideUnitSphere * TTSUtils.Remap(speed, racerTopSpeed * shakeStartEffect, racerTopSpeed, 0, shakiness / 2, true);
        }
        else {
            transform.LookAt(target);
        }

        tiltAngle = Mathf.Lerp(tiltAngle, Mathf.Clamp(target.parent.GetComponent<TTSRacer>().GetTiltAngle(), -Mathf.PI / 2, Mathf.PI / 2), 0.05f);

        transform.RotateAround(transform.forward, tiltAngle);

        if (GetComponent<Vignetting>().chromaticAberration > 0.0f) {
            GetComponent<Vignetting>().chromaticAberration = Mathf.Lerp(GetComponent<Vignetting>().chromaticAberration, 0.0f, 0.07f);
        }
    }

    public void DoDamageEffect() {
        GetComponent<Vignetting>().chromaticAberration = 100;
        transform.position += Random.onUnitSphere * 2;
    }
}