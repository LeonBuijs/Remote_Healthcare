using NUnit.Framework;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SecurityManager.Tests
{
    [TestFixture]
    public class EncryptionTests
    {
        /**
         * Dit is een test waarmee je kunt controleren of de keys goed worden gegenereerd.
         * Er wordt gekeken of de keys niet null zijn en of ze met <RSAKeyValue> beginnen.
         */
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

        /**
         * Dit is een test waarmee je het encrypten en decrypten kunt testen.
         * Eerst wordt de teststring geencrypt en daarna gedecrypt
         * en tot slot wordt er gekeken of dit overeen komt met de begin string.
         */
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

        /**
         * Dit is een test waarmee je het hashen kunt testen.
         * De teststring wordt gehashed met de hashmethode en met een eigen implementatie,
         * deze 2 hashes worden vergeleken.
         */
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