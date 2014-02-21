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
	public Dictionary<float, TTSPowerupNetworkHandle> networkPowerups = new Dictionary<float, TTSPowerupNetworkHandle>();
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

		foreach (KeyValuePair<float, TTSPowerupNetworkHandle> pair in networkPowerups) {
			if (!pair.Value.owner)
				continue;

			UpdateWriter.AddData(TTSOrbsCommands.PowerupUpdate);
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

			//if (DebugMode)
			if(command != 2004 && command != 0 && command != 5004)
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

				case UniGoCommands.OBJECT_IS_NOT_REGISTERED:
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

				// 5001
				case TTSOrbsCommands.PowerupRegister:
					objID = reader.ReadFloat();
					int powerupType = (int)reader.ReadUInt32();
					float racerID = reader.ReadFloat();

					SpawnPowerup(objID, powerupType, racerID);
					break;

				case TTSOrbsCommands.PowerupRegisterOK:
					objID = reader.ReadFloat();
					break;

				case TTSOrbsCommands.PowerupUpdate:
					objID = reader.ReadFloat();
					networkPowerups[objID].NetworkUpdate(reader.ReadVector3(), reader.ReadVector3(), reader.ReadVector3());
					break;

				case TTSOrbsCommands.PowerupDeregister:
					objID = reader.ReadFloat();
					networkPowerups[objID].explode = true;
					break;

				case TTSOrbsCommands.PowerupIsNotRegistered:
				case TTSOrbsCommands.PowerupAlreadyRegistered:
					objID = reader.ReadFloat();
					if (networkPowerups[objID] != null)
						networkPowerups[objID].owner = false;
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

	private void SpawnPowerup(float id, int type, float racerID) {
		networkRacers[racerID].NetworkPowerup(id, type);
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
		UpdateWriter.AddData(TTSOrbsCommands.RACER_DEREGISTER);
		UpdateWriter.AddData(racer.id);
		networkRacers.Remove(racer.id);
	}

	public void RegisterPowerup(TTSPowerupNetworkHandle powerup) {
		if (powerup.owner) {
			do {
				powerup.id = UnityEngine.Random.value * 100;
			} while (networkPowerups.ContainsKey(powerup.id));
		}

		networkPowerups.Add(powerup.id, powerup);

		if (!powerup.owner)
			return;

		//if (DebugMode)
		Debug.Log("X	REGISTERING POWERUP " + powerup.id);

		UpdateWriter.AddData(TTSOrbsCommands.PowerupRegister);
		UpdateWriter.AddData(powerup.id);
		UpdateWriter.AddData(powerup.powerupType);
		UpdateWriter.AddData(powerup.racerID);
	}

	public void DeregisterPowerup(TTSPowerupNetworkHandle powerup) {
		UpdateWriter.AddData(TTSOrbsCommands.PowerupDeregister);
		UpdateWriter.AddData(powerup.id);
		networkPowerups.Remove(powerup.id);
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

public class TTSOrbsCommands : UniGoCommands
{
	public const int RACER_UPDATE = 2004;
	public const int RACER_REGISTER = 2010;
	public const int RACER_REGISTER_OK = 2011;
	public const int RACER_DEREGISTER = 2012;
	public const int RACER_ALREADY_REGISTERED = 2091;
	public const int RACER_IS_NOT_REGISTERED = 2092;

	public const int PowerupRegister = 5001;
	public const int PowerupRegisterOK = 5011;
	public const int PowerupUpdate = 5004;
	public const int PowerupDeregister = 5009;
	public const int PowerupAlreadyRegistered = 5091;
	public const int PowerupIsNotRegistered = 5092;
}