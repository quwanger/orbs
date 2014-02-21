using UnityEngine;
using System.Collections;

public class TTSNetworkObj : TTSBehaviour
{
	UniGoNetworkHandle netHandle;
	public TTSClient client;

	// Use this for initialization
	void Start() {
		client = level.client;
		netHandle = new UniGoNetworkHandle(client, gameObject.transform.position);
	}
	
	// Update is called once per frame
	void Update() {
		if (netHandle.owner) {
			netHandle.Update(transform.position, transform.rotation.eulerAngles, rigidbody.velocity);
		}
		else {
			rigidbody.useGravity = false;
			rigidbody.detectCollisions = false;
			rigidbody.freezeRotation = true;
			rigidbody.isKinematic = true;
		}

		ReceiveNetworkPosition();
	}

	public void ReceiveNetworkPosition() {
		if (netHandle.updated && !netHandle.owner) {
			netHandle.StartRead();
			transform.position = Vector3.Lerp(transform.position, netHandle.networkPos, netHandle.networkInterpolation);
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(netHandle.networkRotation), netHandle.networkInterpolation * 10);
			rigidbody.velocity = netHandle.networkSpeed;
			netHandle.EndRead();
		}
	}
}
