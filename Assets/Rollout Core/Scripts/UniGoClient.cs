using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;

public class UniGoClient : MonoBehaviour
{
	public bool DebugMode = false;
	public string SERVER_IP = "127.0.0.1";//"192.168.1.21";

	// Status
	public bool isConnectionEstablished = false;

	// Sender
	protected UniGoPacketWriter UpdateWriter = new UniGoPacketWriter();
	protected Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
	protected IPAddress serverAddr;
	protected IPEndPoint endPoint;

	// Game Object Updater
	protected Dictionary<float, UniGoNetworkHandle> networkObjects = new Dictionary<float, UniGoNetworkHandle>();

	// Receiver
	protected Thread receiveThread;
	protected bool isRunning = true;
	protected UdpClient client;
	protected UniGoPacketReader reader;

	// Use this for initialization
	void Start() {
		// Sender
		serverAddr = IPAddress.Parse(SERVER_IP);
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

	// Pulls data from objects and sends it
	void Update() {
		// Parse all objects and get all bytes to send as a packet.
		if (isConnectionEstablished == false)
			return;

		foreach (KeyValuePair<float, UniGoNetworkHandle> pair in networkObjects) {
			if (!pair.Value.owner)
				continue;

			UpdateWriter.AddData(UniGoCommands.OBJECT_UPDATE);
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
		UpdateWriter.AddData(UniGoCommands.CLOSE_CONNECTION);
		SendPacket(UpdateWriter);

		client.Client.Close();
	}

	protected void InitConnection() {
		// Send packet with #1
		UniGoPacketWriter packet = new UniGoPacketWriter();
		packet.AddData(UniGoCommands.HANDSHAKE_START);
		SendPacket(packet);
	}

	protected void PacketListener() {
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

	protected void PacketHandler(byte[] data) {
		reader = new UniGoPacketReader(data);
		bool reading = true;
		UniGoPacketWriter writer = new UniGoPacketWriter();

		while (reading) {
			int command = 0;

			if (reader.IsEOF() == false)
				command = (int)reader.ReadUInt32();

			Debug.Log("Received Command: " + command);

			float objID;

			switch (command) {
				case UniGoCommands.END_PACKET:
					reading = false;
					break;
				case UniGoCommands.HANDSHAKE_ACKNOWLEDGE:
					// Server connection established
					writer.AddData(UniGoCommands.HANDSHAKE_COMPLETE);
					isConnectionEstablished = true;
					break;

				case UniGoCommands.OBJECT_UPDATE:
					objID = reader.ReadFloat();
					networkObjects[objID].NetworkUpdate(reader.ReadVector3(), reader.ReadVector3(), reader.ReadVector3());
					break;

				case UniGoCommands.OBJECT_ALREADY_REGISTERED:
					objID = reader.ReadFloat();
					networkObjects[objID].owner = false;
					Debug.Log("Server object already registered: " + objID);
					break;

				default:
					Debug.Log("Invalid Command received: " + command);
					break;
			}
		}
		SendPacket(writer);
	}

	public void RegisterObject(UniGoNetworkHandle obj) {
		networkObjects.Add(obj.id, obj);

		// Send data in the next packet to register
		UpdateWriter.AddData(UniGoCommands.OBJECT_REGISTER);
		UpdateWriter.AddData(obj.id);
	}

	protected void SendPacket(UniGoPacketWriter writer) {
		if (!writer.hasData)
			return;

		writer.AddData(UniGoCommands.END_PACKET);

		if (DebugMode)
			Debug.Log("Sending " + writer.Length + " bytes to " + endPoint.ToString());

		sock.SendTo(writer.GetMinimizedData(), endPoint);
	}

	protected void PrintBytes(byte[] bytes) {
		string str = "";
		foreach (byte b in bytes) {
			str += b.ToString() + " ";
		}
		if (DebugMode)
			Debug.Log(str);
	}
}

public class UniGoCommands
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

public class UniGoNetworkHandle
{
	UniGoClient client;
	public float id;
	public bool updated = false;
	public bool owner = true;

	public Vector3 pos;
	public Vector3 rotation;
	public Vector3 speed;

	public Vector3 networkPos;
	public Vector3 networkRotation;
	public Vector3 networkSpeed;

	public UniGoNetworkHandle() {
		// Only be used for children
	}

	/// <summary>
	/// Network object that will be updated via the network and also update the network objects
	/// </summary>
	/// <param name="Client">The global network client</param>
	/// <param name="StartingPosition">Used to hash and differentiate between different objects</param>
	public UniGoNetworkHandle(UniGoClient Client, Vector3 StartingPosition) {
		client = Client;
		id = StartingPosition.x * StartingPosition.y * StartingPosition.z;

		client.RegisterObject(this);
	}

	public UniGoNetworkHandle(UniGoClient Client, float ID, bool Owner) {
		client = Client;
		id = ID;
		owner = Owner;

		client.RegisterObject(this);
	}

	public UniGoNetworkHandle(UniGoClient Client, Vector3 StartingPosition, bool Owner) {
		client = Client;
		id = StartingPosition.x * StartingPosition.y * StartingPosition.z;
		owner = Owner;

		client.RegisterObject(this);
	}

	/// <summary>
	/// Update the current obj position here
	/// </summary>
	/// <param name="Position"></param>
	/// <param name="Rotation"></param>
	/// <param name="Speed"></param>
	public void Update(Vector3 Position, Vector3 Rotation, Vector3 Speed) {
		if (!owner)
			return;

		pos = Position;
		rotation = Rotation;
		speed = Speed;
	}

	/// <summary>
	/// Only touched by UniGoClient class
	/// </summary>
	/// <param name="Position"></param>
	/// <param name="Rotation"></param>
	/// <param name="Speed"></param>
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

public class UniGoPacketReader
{
	public byte[] Data;
	public int ReadIndex = 0;

	public UniGoPacketReader(byte[] bytes) {
		Data = bytes;
	}

	public uint ReadUInt32() {
		ReadIndex += 4;
		return BitConverter.ToUInt32(Data, ReadIndex - 4);
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

public class UniGoPacketWriter
{
	public byte[] Data = new byte[1024];
	public int WriteIndex = 0;
	public int Length {
		get { return WriteIndex; }
	}

	public bool hasData {
		get {
			return WriteIndex > 0;
		}
	}

	public UniGoPacketWriter() {
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

	public byte[] GetMinimizedData() {
		byte[] arr = new byte[WriteIndex];
		Buffer.BlockCopy(Data, 0, arr, 0, WriteIndex);
		return arr;
	}

	public string GetBytes() {
		string str = "";

		for (int i = 0; i < WriteIndex; i++) {
			str += Data[i] + ", ";
		}
		return str;
	}
}