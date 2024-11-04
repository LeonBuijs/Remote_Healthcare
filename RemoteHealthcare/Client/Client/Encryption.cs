using System;
using System.Security.Cryptography;
using System.Text;

namespace Client;

/**
 * <summary>
 * Klasse die alle encryptie en decryptie afhandelt van data
 * Kan ook een keypair genereren
 * </summary>
 */
public class Encryption
{
	private static readonly int KeySize = 2048;
	/**
	 * <summary>
	 * Totdat we unit tests hebben zal dit de test methode zijn van Encryption
	 * </summary>
	 */
	public static void TestPrintEncryption()
	{
		var (publicKey, privateKey) = GenerateRsaKeyPair();
		Console.WriteLine($"Public Key: \n{publicKey}\n\nPrivate Key: \n{privateKey}");
		byte[] textToEncrypt = Encoding.ASCII.GetBytes("Hello i am under die water");
		
		byte[] encryptedTextInBytes = EncryptData(textToEncrypt,publicKey);
		Console.WriteLine($"Encrypted Text: \n{Convert.ToBase64String(encryptedTextInBytes)}");
		string decryptedText = DecryptData(encryptedTextInBytes, privateKey);
		Console.WriteLine($"Decrypted Text: \n{decryptedText}");
	}
	
	/**
	 * <summary>
	 * Genereert een publicKey en privateKey
	 * Vervolgens zet hij die in out parameters in de vorm van een string
	 * </summary>
	 * <returns>(string, string) - De publicKey en privateKey. Bij exception null</returns>
	 */
	public static (string, string) GenerateRsaKeyPair()
	{
		try
		{
			using RSA rsa = RSA.Create(KeySize);
			string publicKey = rsa.ToXmlString(false);
			string privateKey = rsa.ToXmlString(true);
			return (publicKey, privateKey);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Failed to generate RSA key: \n{ex}");
			return (null, null);
		}
	}
	
	/**
	 * <summary>
	 * Encrypt de data met RSA en een padding van SHA256
	 * </summary>
	 * <param name="dataToEncrypt">De data die encrypt moet worden</param>
	 * <param name="publicKeyReceiver">De public key van de ontvanger</param>
	 * <returns>byte[] - van de encrypted data. Bij exception </returns>
	 */
	public static byte[] EncryptData(byte[] dataToEncrypt, string publicKeyReceiver)
	{
		try
		{
			using RSA rsa = RSA.Create(KeySize);
			rsa.FromXmlString(publicKeyReceiver);
			return rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA256);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Failed to encrypt data: \n{ex}");
			return null;
		}
	}

	/**
	* <summary>
	* Decrypt de data met RSA en een padding van SHA256
	* </summary>
	* <param name="dataToDecrypt">De data die decrypt moet worden</param>
	* <param name="privateKeyReceiver">De private key van de ontvanger</param>
	* <returns>byte[] - van de decrypted data. Bij exception null</returns>
	*/
	public static string DecryptData(byte[] dataToDecrypt, string privateKeyReceiver)
	{
		try
		{
			using RSA rsa = RSA.Create(KeySize);
			rsa.FromXmlString(privateKeyReceiver);
			return Encoding.UTF8.GetString((rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.OaepSHA256)));
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Failed to decrypt data: \n{ex}");
			return null;
		}

	}

	public static byte[] HashData(string dataToHash)
	{
		return SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(dataToHash));
	}
}