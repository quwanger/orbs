using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;

public class TTSClient : MonoBehaviour
{
	public const string SERVER_IP = "127.0.0.1";//"192.168.1.21";

	// Status
	bool isConnectionEstablished = false;

	// Sender
	TTSPacketWriter UpdateWriter = new TTSPacketWriter();
	Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
	IPAddress serverAddr = IPAddress.Parse(SERVER_IP);
	IPEndPoint endPoint;

	// Game Object Updater
	Dictionary<float, TTSNetworkObj> networkObjects = new Dictionary<float, TTSNetworkObj>();

	// Receiver
	Thread receiveThread;
	bool isRunning = true;
	UdpClient client;
	TTSPacketReader reader;

	// Use this for initialization
	void Start() {
		// Sender
		endPoint = new IPEndPoint(serverAddr, 6666);

		// Receiver
		isRunning = true;
		client = new UdpClient(6969);
		client.Client.ReceiveTimeout = 15000;

		receiveThread = new Thread(new ThreadStart(PacketListener));
		receiveThread.IsBackground = true;
		receiveThread.Start();

		InitConnection();
	}

	// Update is called once per frame
	void Update() {
		// Parse all objects and get all bytes to send as a packet.
		if (isConnectionEstablished == false)
			return;

		foreach (KeyValuePair<float, TTSNetworkObj> pair in networkObjects) {
			if (pair.Value.receiveOnly)
				continue;

			UpdateWriter.AddData(TTSOrbsCommands.OBJECT_UPDATE);
			UpdateWriter.AddData(pair.Value.id);
			UpdateWriter.AddData(pair.Value.pos);
			UpdateWriter.AddData(pair.Value.rotation);
			UpdateWriter.AddData(pair.Value.speed);
		}

		// Send packet if there's something stored.
		if (UpdateWriter.hasData) {
			SendPacket(UpdateWriter);
			UpdateWriter.ClearData();
		}
	}

	void OnApplicationQuit() {
		isRunning = false;

		UpdateWriter.ClearData();
		UpdateWriter.AddData(TTSOrbsCommands.CLOSE_CONNECTION);
		SendPacket(UpdateWriter);

		client.Client.Close();
	}

	private void InitConnection() {
		// Send packet with #1
		TTSPacketWriter packet = new TTSPacketWriter();
		packet.AddData(TTSOrbsCommands.HANDSHAKE_START);
		packet.AddData(transform.position);
		SendPacket(packet);
	}

	private void PacketListener() {
		System.Net.IPEndPoint ep = null;
		while (isRunning) {
			Debug.Log("Start Listening");
			byte[] data = client.Receive(ref ep);

			Thread handler = new Thread(new ThreadStart(
				delegate() {
					PacketHandler(data);
				}
			));
			handler.IsBackground = true;
			handler.Start();
		}

		Debug.Log("ended");
	}

	private void PacketHandler(byte[] data) {
		reader = new TTSPacketReader(data);
		bool reading = true;
		TTSPacketWriter writer = new TTSPacketWriter();

		while (reading) {
			int command = 0;

			if (reader.IsEOF() == false)
				command = (int)reader.ReadUInt32();

			Debug.Log("Received Command: " + command);

			float objID;

			switch (command) {
				case TTSOrbsCommands.END_PACKET:
					reading = false;
					break;
				case TTSOrbsCommands.HANDSHAKE_ACKNOWLEDGE:
					// Server connection established
					writer.AddData(TTSOrbsCommands.HANDSHAKE_COMPLETE);

					// Register objects with server
					//foreach (KeyValuePair<float, TTSNetworkObj> pair in networkObjects)
					//{
					//    writer.AddData(REGISTER_OBJECT);
					//    writer.AddData(pair.Value.id);
					//}
					isConnectionEstablished = true;
					break;

				case TTSOrbsCommands.OBJECT_UPDATE:
					objID = reader.ReadFloat();
					networkObjects[objID].NetworkUpdate(reader.ReadVector3(), reader.ReadVector3(), reader.ReadVector3());
					break;

				case TTSOrbsCommands.OBJECT_ALREADY_REGISTERED:
					objID = reader.ReadFloat();
					networkObjects[objID].receiveOnly = true;
					Debug.Log("Server object already registered: " + objID);
					break;

				default:
					Debug.Log("Invalid Command received: " + command);
					break;
			}
		}
		SendPacket(writer);
	}

	public void RegisterObject(TTSNetworkObj obj) {
		networkObjects.Add(obj.id, obj);

		// Send data in the next packet to register
		UpdateWriter.AddData(TTSOrbsCommands.OBJECT_REGISTER);
		UpdateWriter.AddData(obj.id);
	}

	private void SendPacket(TTSPacketWriter writer) {
		if (!writer.hasData)
			return;

		writer.AddData(TTSOrbsCommands.END_PACKET);

		Debug.Log("Sending " + writer.Data.Length + " bytes to " + endPoint.ToString());
		sock.SendTo(writer.Data, endPoint);
	}

	private void PrintBytes(byte[] bytes) {
		string str = "";
		foreach (byte b in bytes) {
			str += b.ToString() + " ";
		}
		Debug.Log(str);
	}
}

public class TTSOrbsCommands
{
	public const int END_PACKET = 0;
	public const int HANDSHAKE_START = 1;
	public const int HANDSHAKE_ACKNOWLEDGE = 2;
	public const int HANDSHAKE_COMPLETE = 3;
	public const int CLOSE_CONNECTION = 9999;

	public const int OBJECT_UPDATE = 1004;
	public const int OBJECT_REGISTER = 1010;
	public const int OBJECT_REGISTER_OK = 1011;
	public const int OBJECT_ALREADY_REGISTERED = 1091;
	public const int OBJECT_IS_NOT_REGISTERED = 1092;
}

public class TTSNetworkObj
{
	TTSClient client;
	public float id;
	public bool receiveOnly = false;
	public bool updated = false;

	public Vector3 pos;
	public Vector3 rotation;
	public Vector3 speed;

	public Vector3 networkPos;
	public Vector3 networkRotation;
	public Vector3 networkSpeed;

	/// <summary>
	/// Network object that will be updated via the network and also update the network objects
	/// </summary>
	/// <param name="Client">The global network client</param>
	/// <param name="StartingPosition">Used to hash and differentiate between different objects</param>
	public TTSNetworkObj(TTSClient Client, Vector3 StartingPosition) {
		client = Client;
		id = StartingPosition.x * StartingPosition.y * StartingPosition.z;
		client.RegisterObject(this);
	}

	public TTSNetworkObj(TTSClient Client, Vector3 StartingPosition, bool ReceiveOnly) {
		receiveOnly = true;
		client = Client;
		id = StartingPosition.x * StartingPosition.y * StartingPosition.z;
		client.RegisterObject(this);
	}

	public void Update(Vector3 Position, Vector3 Rotation, Vector3 Speed) {
		if (receiveOnly)
			return;

		pos = Position;
		rotation = Rotation;
		speed = Speed;
	}

	public void NetworkUpdate(Vector3 Position, Vector3 Rotation, Vector3 Speed) {

		networkPos = Position;
		networkRotation = Rotation;
		networkSpeed = Speed;

		updated = true;
	}

	public void StartRead() {
	}

	public void EndRead() {
		updated = false;
	}
}

class TTSPacketReader
{
	public byte[] Data;
	public int ReadIndex = 0;

	public TTSPacketReader(byte[] bytes) {
		Data = bytes;
	}

	public uint ReadUInt32() {
		ReadIndex += 4;
		return (uint)BitConverter.ToUInt32(Data, ReadIndex - 4);
	}

	public float ReadFloat() {
		ReadIndex += 4;
		return BitConverter.ToSingle(Data, ReadIndex - 4);
	}

	public Vector3 ReadVector3() {
		return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
	}

	public bool IsEOF() {
		return (Data.Length <= ReadIndex);
	}
}

public class TTSPacketWriter
{
	public byte[] Data = new byte[1024];
	public int WriteIndex = 0;

	public bool hasData {
		get {
			return WriteIndex > 0;
		}
	}

	public TTSPacketWriter() {
	}

	public void AddData(byte[] bytes) {
		Buffer.BlockCopy(bytes, 0, Data, WriteIndex, bytes.Length);
	}

	public void AddData(int num) {
		var arr = new int[] { num };

		Buffer.BlockCopy(arr, 0, Data, WriteIndex, arr.Length * sizeof(int));

		WriteIndex += sizeof(int);
	}

	public void AddData(float num) {
		var arr = new float[] { num };

		Buffer.BlockCopy(arr, 0, Data, WriteIndex, arr.Length * sizeof(float));

		WriteIndex += sizeof(float);
	}

	public void AddData(Vector3 vec) {
		float[] arr = new float[] { vec.x, vec.y, vec.z };

		Buffer.BlockCopy(arr, 0, Data, WriteIndex, arr.Length * sizeof(float));

		WriteIndex += sizeof(float) * arr.Length;
	}

	public void ClearData() {
		Data = new byte[1024];
		WriteIndex = 0;
	}

	public string GetBytes() {
		string str = "";

		for (int i = 0; i < WriteIndex; i++) {
			str += Data[i] + ", ";
		}
		return str;
	}
}