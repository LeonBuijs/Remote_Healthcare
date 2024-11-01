using System.Net.Sockets;
using System.Text;
using Server;

namespace UnitTests;

public class ServerTests
{
    private Server.Server server;
    private TcpClient artsClient;
    private NetworkStream artsStream;

    private TcpClient clientClient;
    private NetworkStream clientStream;

    [SetUp]
    public void Setup()
    {
        server = new Server.Server();
        server.SetCallbacks();

        artsClient = new TcpClient("127.0.0.1", 7777);
        artsStream = artsClient.GetStream();
        
        clientClient = new TcpClient("127.0.0.1", 6666);
        clientStream = clientClient.GetStream();
    }

    [TearDown]
    public void TearDown()
    {
        // Sluit de stream en de client correct af
        artsStream.Close();
        artsClient.Close();
        clientStream.Close();
        clientClient.Close();
    }

    [Test]
    [TestCase("0 test test", "0 1")]
    [TestCase("0 testFout testFout", "0 0")]
    [TestCase("0 TeSt TesT", "0 0")]
    public void ShouldRespondCorrectly_WhenDoctorAttemptsLogin(string input, string expectedResponse)
    {
        // Arrange
        var data = Encoding.ASCII.GetBytes(input);
        artsStream.Write(data, 0, data.Length);

        // Act
        var response = ReadResponseFromStream(artsStream).TrimEnd('\n');

        // Assert
        Assert.AreEqual(expectedResponse, response); // Controleer het verwachte antwoord
    }
    
    [Test]
    [TestCase("0 a a 1", "5 1")]
    [TestCase("0 testFout testFout 11223333", "5 0")]
    [TestCase("0 TeSt TesT 01012000", "5 0")]
    public void ShouldRespondCorrectly_WhenClientAttemptsLogin(string input, string expectedResponse)
    {
        // Arrange
        var data = Encoding.ASCII.GetBytes(input);
        clientStream.Write(data, 0, data.Length);

        // Act
        var response = ReadResponseFromStream(clientStream).TrimEnd('\n');

        // Assert
        Assert.AreEqual(expectedResponse, response); // Controleer het verwachte antwoord
    }
    
    private string ReadResponseFromStream(Stream stream)
    {
        var buffer = new byte[10];
        var bytesRead = stream.Read(buffer, 0, buffer.Length);
        return Encoding.ASCII.GetString(buffer, 0, bytesRead);
    }
}