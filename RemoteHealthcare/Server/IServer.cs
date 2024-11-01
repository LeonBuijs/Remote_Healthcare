namespace Server;

public interface IServer
{
    void DoctorLogin(IConnection connection, string[] messageParts);
    void StartSession(string[] messageParts);
    Task StopSession(string[] messageParts);
    Task EmergencyStop(string[] messageParts);
    void SendChatMessageToClient(string[] messageParts);
    void MessageToAllSessions(string[] messageParts);
    void GetSessionData(Connection connection, string[] messageParts);
    void SendAllClients(IConnection connection);
    void SendLiveData(Connection connection, string[] messageParts);
    void SendClientDisconnected(Connection connection);
    void ClientLogin(Connection connection, string[] messageParts);
    void ReceiveBikeData(Connection connection, string[] messageParts);
    void DisconnectClient(Connection connection);
    string GetIndexClient(string[] messageParts);
    void SendCommandToClient(string[] messageParts, string command);
    Tuple<ClientConnection, string> GetClientConnection(Connection connection);


}
