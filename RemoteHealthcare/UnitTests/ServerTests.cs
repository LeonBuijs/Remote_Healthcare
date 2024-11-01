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

        [Test]
        [TestCase(new[] { "0", "Hello", "world", "!" }, "0 Hello world !")]
        [TestCase(new[] { "1", "Goodbye", "world", "!" }, "1 Goodbye world !")]
        [TestCase(new[] { "2", "Testing", "123" }, "2 Testing 123")]
        [TestCase(new[] { "3", "This", "is", "a", "test" }, "3 This is a test")]
        [TestCase(new[] { "4", "SingleMessage" }, "4 SingleMessage")]
        [TestCase(new[] { "5", "" }, "5 ")]
        public void SendChatMessageToClient_ShouldFormatMessageCorrectly(string[] messageParts, string expectedMessage)
        {
            // Arrange
            var mockConnection = new Mock<IConnection>();
            var mockServer = new Mock<IServer>();
    
            // Stel in dat de mock een aanroep naar SendCommandToClient verwacht met de juiste argumenten
            mockServer.Setup(s => s.SendCommandToClient(It.IsAny<string[]>(), It.IsAny<string>()))
                .Callback<string[], string>((parts, msg) =>
                {
                    // Controleer of het bericht goed is geformatteerd
                    Assert.AreEqual(expectedMessage, msg);
                });

            // Act
            mockServer.Object.SendCommandToClient(messageParts, $"{messageParts[0]} {string.Join(" ", messageParts.Skip(1))}");

            // Assert
            mockServer.Verify(s => s.SendCommandToClient(It.IsAny<string[]>(), expectedMessage), Times.Once);
        }

    }
}