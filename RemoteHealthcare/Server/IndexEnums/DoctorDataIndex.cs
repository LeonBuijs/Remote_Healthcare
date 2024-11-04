namespace Server;

public enum DoctorDataIndex
{
    Login,
    StartCommand,
    StopSession,
    EmergencyStopSession,
    MessageToSession,
    MessageToAllSessions,
    SetBikeSettings,
    RetrieveSessionData,
    RetrieveAllClients,
    AddNewClient,
    RetrieveLiveData,
    Disconnected = 404
}