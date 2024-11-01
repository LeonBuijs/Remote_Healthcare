using NUnit.Framework;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SecurityManager.Tests
{
    [TestFixture]
    public class EncryptionTests
    {
        [Test]
        public void TestGenerateRsaKeyPair()
        {
            // Act
            var (publicKey, privateKey) = Encryption.GenerateRsaKeyPair();

            // Assert
            Assert.IsNotNull(publicKey);
            Assert.IsNotNull(privateKey);
            Assert.IsTrue(publicKey.StartsWith("<RSAKeyValue>"));
            Assert.IsTrue(privateKey.StartsWith("<RSAKeyValue>"));
        }

        [Test]
        [TestCase("Hello, World!")]
        [TestCase("")]
        [TestCase("12345!@#$%^&*()_+-=~")]
        [TestCase("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.")]
        public void TestEncryptAndDecrypt(String text)
        {
            // Arrange
            var (publicKey, privateKey) = Encryption.GenerateRsaKeyPair();
            byte[] textToEncrypt = Encoding.ASCII.GetBytes(text);

            // Act
            byte[] encryptedText = Encryption.EncryptData(textToEncrypt, publicKey);
            string decryptedText = Encryption.DecryptData(encryptedText, privateKey);

            // Assert
            Assert.AreEqual(text, decryptedText);
        }

        [Test]
        [TestCase("Hello, World!")]
        [TestCase("")]
        [TestCase("12345!@#$%^&*()_+-=~")]
        [TestCase("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.")]
        public void TestHashData(string text)
        {
            // Arrange
            byte[] expectedHash = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(text));

            // Act
            byte[] actualHash = Encryption.HashData(text);

            // Assert
            Assert.AreEqual(expectedHash, actualHash);
        }
    }
}