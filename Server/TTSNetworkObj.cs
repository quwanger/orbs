using UnityEngine;
using System.Collections;

public class TTSNetworkObj : MonoBehaviour
{
	TTSNetworkHandle networker;
	public TTSClient client;

	// Use this for initialization
	void Start() {
		networker = new TTSNetworkHandle(client, gameObject.transform.position);
	}
	
	// Update is called once per frame
	void Update() {
		if (networker.owner) {
			networker.Update(transform.position, transform.rotation.eulerAngles, rigidbody.velocity);
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
		if (networker.updated && !networker.owner) {
			networker.StartRead();
			transform.position = networker.networkPos;
			transform.rotation = Quaternion.Euler(networker.networkRotation);
			rigidbody.velocity = networker.networkSpeed;
			networker.EndRead();
		}
	}
}
