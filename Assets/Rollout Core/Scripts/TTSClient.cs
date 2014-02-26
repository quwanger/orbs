using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;

public class TTSClient : MonoBehaviour
{
	#region Configuration
	public bool DebugMode = true;

	public string SERVER_IP = "127.0.0.1";
	public int SERVER_RECEIVE_PORT = 6666;
	public int CLIENT_RECEIVE_PORT = 6969;
	public int CLIENT_RECEIVE_TIMEOUT = 15000;
	#endregion

	#region Network
	private IPAddress serverAddr;
	private IPEndPoint endPoint;
	private Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
	private UdpClient client;
	private Thread receiveThread;
	private bool isRunning = true;
	#endregion

	// Status
	public bool EnteredLobby = false;
	public bool InGame = false;
	public int LobbyID = 0;

	// Game networking
	// One global collection of IDs and handles
	Dictionary<float, TTSNetworkHandle> netHandles = new Dictionary<float, TTSNetworkHandle>();
	TTSPacketWriter UpdatePacket = new TTSPacketWriter();

	// Use this for initialization
	void Start() {
		serverAddr = IPAddress.Parse(SERVER_IP);
		endPoint = new IPEndPoint(serverAddr, SERVER_RECEIVE_PORT);

		client = new UdpClient(CLIENT_RECEIVE_PORT);
		client.Client.ReceiveTimeout = 15000;

		receiveThread = new Thread(new ThreadStart(PacketListener));
		receiveThread.IsBackground = true;
		receiveThread.Start();

		InitConnection();
	}

	private void InitConnection() {
		TTSPacketWriter packet = new TTSPacketWriter();
		packet.AddData(TTSCommandTypes.LobbyRegister);
		packet.AddData(1);
		SendPacket(packet);
	}

	// Unity update. Used to send values
	void Update() {
		if (!EnteredLobby) {
			foreach (KeyValuePair<float, TTSNetworkHandle> pair in netHandles) {
				pair.Value.writer.ClearData();
			}
			return;
		}

		foreach (KeyValuePair<float, TTSNetworkHandle> pair in netHandles) {
			TTSNetworkHandle handle = pair.Value;
			UpdatePacket.AddData(handle.GetNetworkUpdate());
		}
	}
	void OnApplicationQuit() {
		isRunning = false;

		UpdatePacket.ClearData();
		UpdatePacket.AddData(TTSCommandTypes.CloseConnection);
		SendPacket(UpdatePacket);

		client.Client.Close();
	}

	// Listens for a packet and starts a new thread to handle the packet
	public void PacketListener() {
		System.Net.IPEndPoint ep = null;
		while (isRunning) {
			byte[] data = client.Receive(ref ep);

			Thread handler = new Thread(new ThreadStart(
			delegate() {
				PacketHandler(data);
			}
			));
			handler.IsBackground = true;
			handler.Start();
		}
	}

	public void PacketHandler(byte[] data) {
		TTSPacketReader packet = new TTSPacketReader(data);

		int command = packet.ReadInt32();

		while (command != TTSCommandTypes.EndPacket) {
			if (DebugMode) Debug.Log(">	Received command " + command);

			switch (command) {
				case TTSCommandTypes.LobbyRegisterOK:
					LobbyID = packet.ReadInt32();
					EnteredLobby = true;
					break;
			}

			command = packet.ReadInt32();
		}
	}

	public void LocalObjectRegister(TTSNetworkHandle handler) {
		if (handler.canForfeitControl) {
			if (netHandles.ContainsKey(handler.id)) // Someone else is controlling object
				handler.owner = false;
		}
		else { // If the object must be controlled, generate a new non-zero key
			while (netHandles.ContainsKey(handler.id) && handler.id == 0.0f) {
				handler.id = UnityEngine.Random.value * 100;
			}
		}

		netHandles.Add(handler.id, handler);
	}

	private void SendPacket(TTSPacketWriter writer) {
		if (!writer.hasData)
			return;

		writer.AddData(TTSCommandTypes.EndPacket);

		if (DebugMode)
			Debug.Log("Sending " + writer.Length + " bytes to " + endPoint.ToString());

		sock.SendTo(writer.GetMinimizedData(), endPoint);
	}
}

public static class TTSCommandTypes
{
	// Connection
	public const int EndPacket            = 0;
	public const int HandshakeStart       = 1;
	public const int HandshakeAcknowledge = 2;
	public const int HandshakeComplete    = 3;
	public const int Ping                 = 4;
	public const int Pong                 = 5;
	public const int CloseConnection      = 9999;

	// Lobby
	public const int LobbyRegister     = 100;
	public const int LobbyRegisterOK   = 101;
	public const int LobbyRegisterFail = 109;

	// Object
	public const int ObjectUpdate            = 1004;
	public const int ObjectRegister          = 1010;
	public const int ObjectRegisterOK        = 1011;
	public const int ObjectAlreadyRegistered = 1091;
	public const int ObjectIsNotRegistered   = 1092;

	// Racer
	public const int RacerUpdate            = 2004;
	public const int RacerRegister          = 2010;
	public const int RacerRegisterOK        = 2011;
	public const int RacerDeregister        = 2012;
	public const int RacerAlreadyRegistered = 2091;
	public const int RacerIsNotRegistered   = 2092;

	// Powerup
	public const int PowerupRegister          = 5001;
	public const int PowerupRegisterOK        = 5011;
	public const int PowerupUpdate            = 5004;
	public const int PowerupDeregister        = 5009;
	public const int PowerupAlreadyRegistered = 5091;
	public const int PowerupIsNotRegistered   = 5092;

	// General
	public const int RequestNumRacers = 8001;
	public const int ReturnNumRacers = 8002;
}

public abstract class TTSNetworkHandle
{
	protected TTSClient client;
	public float id; // ID will only be stored here
	public TTSPacketWriter writer = new TTSPacketWriter(); // Each object must use this writer to write packet data
	public bool canForfeitControl = false; // Whether object can be taken control of by another client (for scene objects)

	public bool owner;

	public TTSNetworkHandle() {

	}

	// You must override this method.
	public abstract void ReceiveNetworkData(TTSPacketReader reader);

	// Do not override this method unless necessary
	public byte[] GetNetworkUpdate() {
		return writer.GetMinimizedData();
	}
}

public class TTSPacketReader
{
	public byte[] Data;
	public int ReadIndex = 0;

	public TTSPacketReader(byte[] bytes) {
		Data = bytes;
	}

	public uint ReadUInt32() {
		ReadIndex += 4;
		return BitConverter.ToUInt32(Data, ReadIndex - 4);
	}

	public int ReadInt32() {
		return (int)ReadUInt32();
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

	public string GetBytes() {
		string str = "";

		for (int i = 0; i < Data.Length; i++) {
			str += Data[i] + ", ";
		}
		return str;
	}
}

public class TTSPacketWriter
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