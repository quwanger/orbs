package OrbsCommandTypes

const (

	// Connection
	EndPacket            = 0
	HandshakeStart       = 1
	HandshakeAcknowledge = 2
	HandshakeComplete    = 3
	Ping                 = 4
	Pong                 = 5
	CloseConnection      = 9999

	// Lobby
	LobbyRegister     = 100
	LobbyRegisterOK   = 101
	LobbyRegisterFail = 109
	LobbyInfoRequest  = 110
	LobbyInfoReturn   = 111

	LobbyStartGame       = 140
	LobbyEndGame         = 141
	LobbyCountdownUpdate = 124
	LobbyStopCountdown   = 125
	LobbyGiveAIRacer     = 130

	RaceStartCountdown = 150
	RaceStartReady     = 160

	// Object Control
	GiveControl = 1111

	// Object
	ObjectUpdate            = 1004
	ObjectRegister          = 1010
	ObjectRegisterOK        = 1011
	ObjectAlreadyRegistered = 1091
	ObjectIsNotRegistered   = 1092

	// Racer
	RacerUpdate            = 2004
	RacerRegister          = 2010
	RacerRegisterOK        = 2011
	RacerDeregister        = 2012
	RacerReadyState        = 2020
	RacerAlreadyRegistered = 2091
	RacerIsNotRegistered   = 2092

	// Powerup
	PowerupRegister          = 5001
	PowerupStaticRegister    = 5002
	PowerupRegisterOK        = 5011
	PowerupUpdate            = 5004
	PowerupDeregister        = 5009
	PowerupAlreadyRegistered = 5091
	PowerupIsNotRegistered   = 5092

	// PowerupPlatform
	PowerupPlatformRegister          = 5501
	PowerupPlatformRegisterOK        = 5511
	PowerupPlatformSpawn             = 5508
	PowerupPlatformPickedUp          = 5505
	PowerupPlatformAlreadyRegistered = 5591

	// General
	RequestNumRacers  = 8001
	ReturnNumRacers   = 8002
	RequestAllLobbies = 8005
	ReturnAllLobbies  = 8006

	//What?
	Dislodger = 123123123
)
