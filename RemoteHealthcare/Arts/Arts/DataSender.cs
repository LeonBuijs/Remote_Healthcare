using System.IO;
using System.Text;
using SecurityManager;

namespace Arts;

/**
 * Deze klasse handelt het versturen van de data af voor de artsClient
 */
public class DataSender
{
    private Stream stream;
    public string publicServerKey { set; get; }
    public DataSender(Stream stream)
    {  
        this.stream = stream;
    }

    private void Write(string finalString, bool encryption = true)
    {
        Console.WriteLine($"Dokter sending: {finalString}");
        try
        {
            byte[] bytesToSend = Encoding.ASCII.GetBytes(finalString);

            if (encryption)
            {
                bytesToSend = Encryption.EncryptData(bytesToSend, publicServerKey);
                //vang de mogelijke null return waarde af
                if (bytesToSend == null)
                {
                    Console.WriteLine("Failed to encrypt data. Aborting send.");
                    return;
                }
            }
            stream.Write(bytesToSend);
            stream.Flush();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Couldn't write to server with exception: {exception}");
        }
        
    }

    public void SendPublicKey(string publicKey)
    {
        Write(publicKey, false);
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

    public void GetClients()
    {
        Write("8");
    }
    
    public void MakeClient(string clientInfo)
    {
        Write($"9 {clientInfo} ");
    }
    
    public void ChosenClient(string clientInfo)
    {
        Write($"10 {clientInfo} ");
    }
}