using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class TTSNetworking : MonoBehaviour
{
	Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

	IPAddress serverAddr = IPAddress.Parse("192.168.1.233");
	IPEndPoint endPoint;

	int i = 0;

	// Use this for initialization
	void Start () {
		endPoint = new IPEndPoint(serverAddr, 6666);
	}
	
	// Update is called once per frame
	void Update() {
		string text = "Hello " + i;
		byte[] send_buffer = Encoding.ASCII.GetBytes(text);

		sock.SendTo(send_buffer, endPoint);
		i++;
	}
}
