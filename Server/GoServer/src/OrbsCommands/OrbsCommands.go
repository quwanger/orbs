package OrbsCommands

const (
	EndPacket            = 0
	HandshakeStart       = 1
	HandshakeAcknowledge = 2
	HandshakeComplete    = 3
	CloseConnection      = 9999

	ObjectUpdate            = 1004
	ObjectRegister          = 1010
	ObjectRegisterOK        = 1011
	ObjectAlreadyRegistered = 1091
	ObjectIsNotRegistered   = 1092

	RacerUpdate            = 2004
	RacerRegister          = 2010
	RacerRegisterOK        = 2011
	RacerDeregister        = 2012
	RacerAlreadyRegistered = 2091
	RacerIsNotRegistered   = 2092
)
