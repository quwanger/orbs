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
	public bool DebugRacerSpawn = true;

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

	TTSLevel level;
	TTSInitRace initRace;

	// Status
	public bool isMultiplayer{
		get{
			return level.currentGameType == TTSLevel.Gametype.MultiplayerOnline;
		}
	}
	public bool isLobby {
		get {
			return level.currentGameType == TTSLevel.Gametype.Lobby;
		}
	}
	public bool EnteredLobby = false;
	public int LobbyID = 0;

	// Game networking
	// One global collection of IDs and handles
	Dictionary<float, TTSNetworkHandle> netHandles = new Dictionary<float, TTSNetworkHandle>();
	Dictionary<float, TTSNetworkHandle> racerHandles = new Dictionary<float, TTSNetworkHandle>();
	TTSPacketWriter UpdatePacket = new TTSPacketWriter();

	List<TTSRacer.RacerConfig> spawnRacers = new List<TTSRacer.RacerConfig>();
	List<TTSPowerupNetHandler> spawnPowerups = new List<TTSPowerupNetHandler>();

	public List<TTSRacer.RacerConfig> RegisteredRacerConfigs = new List<TTSRacer.RacerConfig>();

	// Use this for initialization
	void Start() {
		level = GetComponent<TTSLevel>();
		initRace = GetComponent<TTSInitRace>();

		if (!isMultiplayer && !isLobby) return;

		serverAddr = IPAddress.Parse(SERVER_IP);
		endPoint = new IPEndPoint(serverAddr, SERVER_RECEIVE_PORT);

		client = new UdpClient(CLIENT_RECEIVE_PORT);
		client.Client.ReceiveTimeout = CLIENT_RECEIVE_TIMEOUT;

		receiveThread = new Thread(new ThreadStart(PacketListener));
		receiveThread.IsBackground = true;
		receiveThread.Start();

		InitConnection();
	}

	private void InitConnection() {
		ConnectToLobby(1);
		//RequestLobbyInfo();
	}

	public void RequestLobbyInfo() {
		TTSPacketWriter packet = new TTSPacketWriter();
		packet.AddData(TTSCommandTypes.RequestAllLobbies);
		SendPacket(packet);
	}

	public void ReceiveLobbyInfo(TTSPacketReader reader) {
		int numLobbies = reader.ReadInt32();
		Debug.Log(numLobbies + " Lobbies. " + reader.Data.Length);

		List<TTSLobby> lobbies = new List<TTSLobby>();

		for (int i = 0; i < numLobbies; i++) {
			int lobbyID = reader.ReadInt32();
			string lobbyName = reader.Read16CharString();
			int numRacers = reader.ReadInt32();
			int maxRacers = reader.ReadInt32();
			bool inGame = reader.ReadBool();
			bool botsEnabled = reader.ReadBool();
			int levelID = reader.ReadInt32();

			lobbies.Add(new TTSLobby(lobbyID, lobbyName, numRacers, maxRacers, inGame, botsEnabled, levelID));
		}
	}

	public void ConnectToLobby(int lobby) {
		TTSPacketWriter packet = new TTSPacketWriter();
		packet.AddData(TTSCommandTypes.LobbyRegister);
		packet.AddData(lobby);
		SendPacket(packet);
	}

	// Unity update. Used to send values
	void Update() {
		if (!EnteredLobby || !isMultiplayer) {
			foreach (KeyValuePair<float, TTSNetworkHandle> pair in netHandles) {
				pair.Value.writer.ClearData();
			}
			return;
		}

		// Code to run during lobby
		foreach (KeyValuePair<float, TTSNetworkHandle> pair in netHandles) {
			TTSNetworkHandle handle = pair.Value;
			if (handle.isServerRegistered) {
				// First make sure the packet won't overflow
				byte[] tempData = handle.GetNetworkUpdate();
				if (UpdatePacket.WillOverflow(tempData.Length)) {
					SendPacket(UpdatePacket, true);
				}
				UpdatePacket.AddData(tempData);
			}
		}
		SendPacket(UpdatePacket, true);

		if (!isMultiplayer)
			return;
		// Code to run during in game.

		// Spawn multiplayer racers
		if (spawnRacers.Count > 0) {
			foreach (TTSRacer.RacerConfig config in spawnRacers) {
				initRace.InitMultiplayerRacer(config);
			}
			spawnRacers.Clear();
		}
	}
	void OnApplicationQuit() {
		isRunning = false;

		if (isMultiplayer || isLobby) {
			UpdatePacket.ClearData();
			UpdatePacket.AddData(TTSCommandTypes.CloseConnection);
			SendPacket(UpdatePacket);

			client.Client.Close();
		}
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
		float id = -1;

		while (command != TTSCommandTypes.EndPacket) {
			//if (DebugMode)
				Debug.Log(">	Received command " + command);

			switch (command) {

				#region Lobby
				case TTSCommandTypes.LobbyRegisterOK:
					LobbyID = packet.ReadInt32();
					EnteredLobby = true;
					ServerAllObjectsRegister();
					break;

				case TTSCommandTypes.ReturnAllLobbies:
					ReceiveLobbyInfo(packet);

					break;

				#endregion

				#region racers and powerups
				case TTSCommandTypes.RacerRegister:
					TTSRacer.RacerConfig config = new TTSRacer.RacerConfig();
					id = config.netID = packet.ReadFloat();
					config.Index = packet.ReadInt32();
					config.RigType = packet.ReadInt32();
					config.Perk1 = packet.ReadInt32();
					config.Perk2 = packet.ReadInt32();
					config.Name = packet.Read16CharString();
					config.ControlType = packet.ReadInt32();
					config.LocalControlType = TTSUtils.EnumToInt(TTSRacer.PlayerType.Multiplayer);
					RegisteredRacerConfigs.Add(config);

					if (DebugRacerSpawn){
						Debug.Log("R	Received a racer " + id);
						spawnRacers.Add(config);
					}
					break;

				case TTSCommandTypes.RacerUpdate:
				case TTSCommandTypes.PowerupStaticRegister:
				case TTSCommandTypes.PowerupRegister:
				case TTSCommandTypes.PowerupUpdate:
					id = packet.ReadFloat();
					netHandles[id].ReceiveNetworkData(packet, command);
					break;

				case TTSCommandTypes.RacerRegisterOK:
					id = packet.ReadFloat();
					netHandles[id].isServerRegistered = true;
					int index = packet.ReadInt32(); // Starting point index
					break;

				case TTSCommandTypes.PowerupRegisterOK:
					id = packet.ReadFloat();
					netHandles[id].isServerRegistered = true;
					break;

				case TTSCommandTypes.RacerDeregister:
					id = packet.ReadFloat();
					Debug.Log("DEREGISTER RACER " + id);
					// IMPLMENTE THIS
					break;

				case TTSCommandTypes.RacerAlreadyRegistered:
					// Shouldn't really happen...
					break;
				#endregion
			}

			if (packet.IsEOF()) {
				command = TTSCommandTypes.EndPacket;
			}
			else {
				command = packet.ReadInt32();
			}
		}
	}

	#region InGame
	// Send all the registered objects
	private void ServerAllObjectsRegister() {
		TTSPacketWriter writer = new TTSPacketWriter();
		foreach (KeyValuePair<float, TTSNetworkHandle> pair in netHandles) {
			if(pair.Value.owner)
				ServerObjectRegister(pair.Value, writer);
		}
		SendPacket(writer);
	}

	// Writes the necessary register code to the given packet writer
	private void ServerObjectRegister(TTSNetworkHandle handle, TTSPacketWriter writer) {
		byte[] data = handle.GetNetworkRegister();

		if (writer.WillOverflow(data.Length)) { // Overflow check
			SendPacket(writer, true);
		}

		writer.AddData(data);
	}

	public void LocalRacerRegister(TTSRacerNetHandler handler) {
		LocalObjectRegister(handler);
		racerHandles.Add(handler.id, handler);
	}

	public void LocalObjectRegister(TTSNetworkHandle handler) {
		if (handler.canForfeitControl) {
			if (netHandles.ContainsKey(handler.id)) // Someone else is controlling object
				handler.owner = false;
		}
		else { // If the object must be controlled, generate a new non-zero key
			while (netHandles.ContainsKey(handler.id) || handler.id == 0.0f && handler.owner) {
				handler.id = UnityEngine.Random.value * 100;
			}
		}
		if(DebugMode)
			Debug.Log("Registering " + handler.id);

		if (handler.owner) {
			ServerObjectRegister(handler, UpdatePacket);
		}

		netHandles.Add(handler.id, handler);
	}

	public void LocalObjectDeregister(float id) {
		netHandles.Remove(id);
	}

	#endregion

	public bool IsIDTaken(float id) {
		return netHandles.ContainsKey(id);
	}

	private void SendPacket(TTSPacketWriter writer) {
		if (!writer.hasData)
			return;

		writer.AddData(TTSCommandTypes.EndPacket);

		if (DebugMode)
			Debug.Log("Sending " + writer.Length + " bytes to " + endPoint.ToString());

		sock.SendTo(writer.GetMinimizedData(), endPoint);
	}

	private void SendPacket(TTSPacketWriter writer, bool clear) {
		SendPacket(writer);
		if(clear) writer.ClearData();
	}
}

public class TTSLobby
{
	public TTSLevel.LevelType Level;

	public int ID = -1;
	public string Name = "";
	public int NumRacers = 0;
	public int MaxNumRacers = 6;
	public bool InProgress = false;
	public bool BotsEnabled = true;
	public int LevelID {
		get {
			return (int)Level;
		}
		set {
			SetLevel(value);
		}
	}

	public TTSLobby(int id, string name, int numRacers, int maxRacers, bool inProgress, bool botsEnabled, int levelID) {
		ID = id;
		Name = name;
		NumRacers = numRacers;
		MaxNumRacers = maxRacers;
		InProgress = inProgress;
		BotsEnabled = botsEnabled;
		LevelID = levelID;

		Debug.Log("Lobby Received " + id + " " + Name);
	}

	public void SetLevel(int levelID) {
		Level = (TTSLevel.LevelType)levelID;
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
	public const int PowerupRegister		  = 5001;
	public const int PowerupStaticRegister	  = 5002;
	public const int PowerupRegisterOK        = 5011;
	public const int PowerupUpdate            = 5004;
	public const int PowerupDeregister        = 5009;
	public const int PowerupAlreadyRegistered = 5091;
	public const int PowerupIsNotRegistered   = 5092;

	// General
	public const int RequestNumRacers = 8001;
	public const int ReturnNumRacers = 8002;
	public const int RequestAllLobbies = 8005;
	public const int ReturnAllLobbies = 8006;
}

public abstract class TTSNetworkHandle
{
	public string type = "Net Handle";
	public int registerCommand; // Must be set
	public bool isServerRegistered = false;
	public float id = 0.0f; // ID will only be stored here
	public bool canForfeitControl = false; // Whether object can be taken control of by another client (for scene objects)
	public float networkInterpolation = 0.05f;
	public bool inGameRegistration = false; // Whether it should be registered in game or in lobby.

	protected TTSClient client;
	public TTSPacketWriter writer = new TTSPacketWriter(); // Each object must use this writer to write packet data

	public bool owner;
	public bool isWriterUpdated = false;
	public bool isNetworkUpdated = false;

	public TTSNetworkHandle() {
		// Register yourself to the client from here.
	}

	// You must override this method. Command and ID will already be read
	public abstract void ReceiveNetworkData(TTSPacketReader reader, int command);

	public virtual byte[] GetNetworkRegister() {
		writer.AddData(registerCommand);
		writer.AddData(id);
		byte[] data = writer.GetMinimizedData();
		writer.ClearData();
		return data;
	}

	// Do not override this method unless necessary
	public virtual byte[] GetNetworkUpdate() {
		isWriterUpdated = false;
		byte[] data = writer.GetMinimizedData();
		writer.ClearData();
		return data;
	}

	public virtual void DeregisterFromClient() {
		client.LocalObjectDeregister(id);
	}

	public void DoneReading() {
		isNetworkUpdated = false;
	}
}

#region packets
public class TTSPacketReader
{
	public byte[] Data;
	public int ReadIndex = 0;

	public TTSPacketReader(byte[] bytes) {
		Data = bytes;
	}

	public bool ReadBool() {
		return Convert.ToBoolean(Data[ReadIndex++]);
	}

	public string Read16CharString() {
		return ReadString(16);
	}

	public string ReadString(int len) {
		ReadIndex += len;
		return System.Text.Encoding.UTF8.GetString(Data, ReadIndex-len, len);
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

	public void AddData(string str) {
		AddData(System.Text.Encoding.UTF8.GetBytes(str));
	}

	public void AddData(bool b) {
		Data[WriteIndex] = Convert.ToByte(b);
		WriteIndex++;
	}

	public void AddData(byte[] bytes) {
		Buffer.BlockCopy(bytes, 0, Data, WriteIndex, bytes.Length);
		WriteIndex += bytes.Length;
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

	public void AddData(string str, int size) {
		byte[] temp = new byte[size], bytes = System.Text.Encoding.UTF8.GetBytes(str);
		Buffer.BlockCopy(bytes, 0, temp, 0, bytes.Length);
		AddData(temp);
	}

	public void ClearData() {
		Data = new byte[1024];
		WriteIndex = 0;
	}

	public bool WillOverflow(int size) {
		return (WriteIndex + size) >= 1024;
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
#endregion