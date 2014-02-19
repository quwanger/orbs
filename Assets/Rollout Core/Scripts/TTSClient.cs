using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;

public class TTSClient : UniGoClient
{
	public Dictionary<float, TTSRacerNetworkHandle> networkRacers = new Dictionary<float, TTSRacerNetworkHandle>();
	private System.DateTime debugTimeStamp;

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

		foreach (KeyValuePair<float, TTSRacerNetworkHandle> pair in networkRacers) {
			if (!pair.Value.owner)
				continue;

			UpdateWriter.AddData(TTSOrbsCommands.RACER_UPDATE);
			UpdateWriter.AddData(pair.Value.id);
			UpdateWriter.AddData(pair.Value.pos);
			UpdateWriter.AddData(pair.Value.rotation);
			UpdateWriter.AddData(pair.Value.speed);

			UpdateWriter.AddData(pair.Value.vInput);
			UpdateWriter.AddData(pair.Value.hInput);
			UpdateWriter.AddData(pair.Value.powerUpType);
			UpdateWriter.AddData(pair.Value.powerUpTier);
		}

		// Send packet if there's something stored.
		if (UpdateWriter.hasData) {
			SendPacket(UpdateWriter);
			UpdateWriter.ClearData();
		}

		foreach (float id in spawnRacers) {
			GetComponent<TTSInitRace>().InitMultiplayerRacer(id);
		}

		if (spawnRacers.Count > 0) {
			spawnRacers.Clear();
		}
	}
	protected void PacketListener() {
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

		if (DebugMode)
			Debug.Log("ended");
	}

	private List<float> spawnRacers = new List<float>();

	private void PacketHandler(byte[] data) {
		reader = new UniGoPacketReader(data);
		bool reading = true;
		UniGoPacketWriter writer = new UniGoPacketWriter();

		while (reading) {
			int command = 0;

			if (reader.IsEOF() == false)
				command = (int)reader.ReadUInt32();

			if (DebugMode)
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
					if (DebugMode)
						Debug.Log("Server object already registered: " + objID);
					break;

				case TTSOrbsCommands.RACER_UPDATE:
					objID = reader.ReadFloat();
					networkRacers[objID].NetworkUpdate(reader.ReadVector3(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadFloat(), reader.ReadFloat(), (int)reader.ReadUInt32(), (int)reader.ReadUInt32());
					break;

				case TTSOrbsCommands.RACER_REGISTER:
					objID = reader.ReadFloat();
					spawnRacers.Add(objID);
					break;

				case TTSOrbsCommands.RACER_REGISTER_OK:
					objID = reader.ReadFloat();
					break;

				case TTSOrbsCommands.RACER_IS_NOT_REGISTERED:
				case TTSOrbsCommands.RACER_ALREADY_REGISTERED:
					objID = reader.ReadFloat();
					DeregisterRacer(networkRacers[objID]);

					networkRacers[objID].id = UnityEngine.Random.value * 100;
					RegisterRacer(networkRacers[objID]);
					if (DebugMode)
						Debug.Log("Server object registered again: " + objID);
					break;

				default:
					if (DebugMode)
						Debug.Log("Invalid Command received: " + command);
					break;
			}
		}
		SendPacket(writer);
		writer.ClearData();
	}
	public void RegisterRacer(TTSRacerNetworkHandle racer) {

		if (racer.owner) {
			do {
				racer.id = UnityEngine.Random.value * 100;
			} while (networkRacers.ContainsKey(racer.id));
		}

		networkRacers.Add(racer.id, racer);

		if (!racer.owner)
			return;

		if (DebugMode)
			Debug.Log("X	REGISTERING RACER " + racer.id);
		// Send data in the next packet to register
		UpdateWriter.AddData(TTSOrbsCommands.RACER_REGISTER);
		UpdateWriter.AddData(racer.id);
	}

	public void DeregisterRacer(TTSRacerNetworkHandle racer) {
		networkRacers.Remove(racer.id);
	}

	public void DebugFPSOutput() {
		if (debugTimeStamp == null)
			return;

		System.DateTime now = System.DateTime.Now;
		System.TimeSpan span = now - debugTimeStamp;

		Debug.Log(1000 / span.TotalMilliseconds + " FPS");
		debugTimeStamp = now;
	}
}

public class TTSRacerNetworkHandle : UniGoNetworkHandle
{
	public float networkInterpolation = 0.05f;
	public TTSRacerNetworkHandle(TTSClient Client, float ID) {
		id = ID;
		owner = true;
		Client.RegisterRacer(this);
	}

	public TTSRacerNetworkHandle(TTSClient Client, float ID, bool Owner) {
		id = ID;
		owner = Owner;
		Client.RegisterRacer(this);
	}

	public float vInput, hInput;
	public int powerUpType, powerUpTier;

	public float networkVInput, networkHInput;
	public int networkPowerUpType, networkPowerUpTier;

	public void Update(Vector3 Position, Vector3 Rotation, Vector3 Speed, float VInput, float HInput, int PowerUpType, int PowerUpTier) {
		if (!owner)
			return;

		pos = Position;
		rotation = Rotation;
		speed = Speed;
		vInput = VInput;
		hInput = HInput;
		powerUpType = PowerUpType;
		powerUpTier = PowerUpTier;
	}

	// Only to be accessed by TTSClient
	public void NetworkUpdate(Vector3 Position, Vector3 Rotation, Vector3 Speed, float VInput, float HInput, int PowerUpType, int PowerUpTier) {

		networkPos = Position;
		networkRotation = Rotation;
		networkSpeed = Speed;
		networkVInput = VInput;
		networkHInput = HInput;
		networkPowerUpType = PowerUpType;
		networkPowerUpTier = PowerUpTier;

		updated = true;
	}
}

public class TTSOrbsCommands : UniGoCommands
{
	public const int RACER_UPDATE = 2004;
	public const int RACER_REGISTER = 2010;
	public const int RACER_REGISTER_OK = 2011;
	public const int RACER_DEREGISTER = 2012;
	public const int RACER_ALREADY_REGISTERED = 2091;
	public const int RACER_IS_NOT_REGISTERED = 2092;
}