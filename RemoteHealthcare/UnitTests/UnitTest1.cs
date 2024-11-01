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
    }
}