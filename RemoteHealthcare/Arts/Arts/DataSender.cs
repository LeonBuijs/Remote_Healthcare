using System.Text;

namespace Arts;

/**
 * Deze klasse handelt het versturen van de data af voor de artsClient
 */
public class DataSender
{
    private Stream stream;
    public DataSender(Stream stream)
    {  
        this.stream = stream;
    }

    private void Write(string finalString)
    {
        stream.Write(Encoding.ASCII.GetBytes(finalString));
        stream.Flush();
    }

    public void SendLogin(string username, string password)
    {
      Write($"0 {username} {password}");
    }

    public void StartSession(string session)
    {  
        Write($"1 {session}");
    }

    public void StopSession(string session)
    {
        Write($"2 {session}");
    }

    public void EmergencyStopSession(string session)
    {
        Write($"3 {session}");
    }
    
    public void SendMessageToSession(string session, string message)
    {
        //todo hoe willen we de sessie splitsen van message?
        Write($"4 {session} {message}");
    }

    public void SendMessageToAllSessions(string message)
    {
        Write($"5 {message}");
    }

    public void SendBikeConfigs(string session, string configs)
    {
        Write($"6 {session} {configs}");
    }

    public void RetrievePreviousSessions(string session)
    {
        Write($"7 {session}");
    }
}