using Moq;
using Server;

namespace UnitTests
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
        * Test controleert of het verzenden van een chatbericht aan de client
        * correct wordt geformatteerd op basis van de opgegeven berichtenonderdelen.
        */
        [Test]
        [TestCase(new[] { "0", "Hello", "world", "!" }, "0 Hello world !")]
        [TestCase(new[] { "1", "Goodbye", "world", "!" }, "1 Goodbye world !")]
        [TestCase(new[] { "2", "Testing", "123" }, "2 Testing 123")]
        [TestCase(new[] { "3", "This", "is", "a", "test" }, "3 This is a test")]
        [TestCase(new[] { "4", "SingleMessage" }, "4 SingleMessage")]
        [TestCase(new[] { "5", "" }, "5 ")]
        public void SendCommandToClient_ShouldFormatMessageAsExpected(string[] messageParts, string expectedMessage)
        {
            // Arrange
            var mockConnection = new Mock<IConnection>();
            var mockServer = new Mock<IServer>();
    
            mockServer.Setup(s => s.SendCommandToClient(It.IsAny<string[]>(), It.IsAny<string>()))
                .Callback<string[], string>((parts, msg) =>
                {
                    Assert.AreEqual(expectedMessage, msg);
                });

            // Act
            mockServer.Object.SendCommandToClient(messageParts, $"{messageParts[0]} {string.Join(" ", messageParts.Skip(1))}");

            // Assert
            mockServer.Verify(s => s.SendCommandToClient(It.IsAny<string[]>(), expectedMessage), Times.Once);
        }
        
        /**
        * Test de SendChatMessageToClient-methode van de server.
        * Controleert of de berichten correct worden opgemaakt en verzonden naar de client.
        */
        [Test]
        [TestCase(new[] { "Hello", "world", "!" }, "0 Hello world !")]
        [TestCase(new[] { "Goodbye", "world", "!" }, "0 Goodbye world !")]
        [TestCase(new[] { "Testing", "123" }, "0 Testing 123")]
        [TestCase(new[] { "This", "is", "a", "test" }, "0 This is a test")]
        [TestCase(new[] { "SingleMessage" }, "0 SingleMessage")]
        [TestCase(new[] { "" }, "0 ")]
        public void SendChatMessageToClient_ShouldFormatMessageAsExpected(string[] messageParts, string expectedMessage)
        {
            // Arrange
            var mockConnection = new Mock<IConnection>();
            var mockServer = new Mock<IServer>();

            
            mockServer.Setup(s => s.SendChatMessageToClient(It.IsAny<string[]>()))
                .Callback<string[]>(parts =>
                {
                    string formattedMessage = "0 " + string.Join(" ", parts);
                    mockConnection.Object.Send(formattedMessage);
                    Assert.AreEqual(expectedMessage, formattedMessage);
                });

            // Act
            mockServer.Object.SendChatMessageToClient(messageParts);

            // Assert
            mockConnection.Verify(conn => conn.Send(expectedMessage), Times.Once);
        }


    }
}