using Moq;
using Server;
using NUnit.Framework;

namespace ServerTests
{
    public class ServerTests
    {
        /**
        * Test controleert of de toegang van de dokter correct wordt ingesteld
        * op basis van de opgegeven inloggegevens.
        */
        [Test]
        [TestCase("test", "test", true)]
        [TestCase("testFout", "test", false)]
        [TestCase("test", "testFout", false)]
        [TestCase("testFout", "testFout", false)]
        public void DoctorLogin_ShouldSetAccessCorrectly(string username, string password, bool expectedAccess)
        {
            // Arrange
            var mockConnection = new Mock<IConnection>();
            mockConnection.SetupProperty(c => c.Access, false);

            var mockServer = new Mock<IServer>();
            mockServer.Setup(s => s.DoctorLogin(It.IsAny<IConnection>(), It.IsAny<string[]>()))
                .Callback<IConnection, string[]>((conn, credentials) => 
                {
                    if (credentials[0] == "test" && credentials[1] == "test")
                    {
                        conn.Access = true;
                    }
                    else
                    {
                        conn.Access = false;
                    }
                });

            // Act
            mockServer.Object.DoctorLogin(mockConnection.Object, new string[] { username, password });

            // Assert
            Assert.AreEqual(expectedAccess, mockConnection.Object.Access);
        }
        
        /**
         * Deze test controleert of de client correct in een sessie wordt gezet
         * wanneer de StartSession-methode wordt aangeroepen met de juiste client-ID.
         */
        [Test]
        [TestCase("client1", true)]
        [TestCase("client2", false)]
        public void StartSession_ShouldSetClientInSession(string clientId, bool expectedInSession)
        {
            // Arrange
            var mockConnection = new Mock<Connection>();
            var mockServer = new Server.Server();
            mockServer.clients.Add(clientId, new ClientConnection(clientId, mockConnection.Object));
            
            var messageParts = new string[] { "1", clientId }; 

            // Act
            mockServer.StartSession(messageParts);

            // Assert
            Assert.AreEqual(expectedInSession, mockServer.clients[clientId].InSession);
        }

        /**
         * Deze test controleert of er niets gebeurt als er geprobeerd wordt
         * een sessie te starten voor een client die niet bestaat.
         */
        [Test]
        [TestCase("nonExistentClient")]
        public void StartSession_ShouldNotSetClientInSession_WhenClientDoesNotExist(string clientId)
        {
            // Arrange
            var mockServer = new Server.Server();
            var messageParts = new string[] { "1", clientId };

            // Act
            mockServer.StartSession(messageParts);

            // Assert
            Assert.That(mockServer.clients.ContainsKey(clientId), Is.False);
        }
    }
}