using System;
using System.Security.Cryptography;
using System.Text;

namespace Client;
/**
 * <summary>
 * Klasse die alle encryptie en decryptie afhandelt van data
 * Kan ook een een public- en private key genereren
 * LET OP! DEZE KLAS IS COMPLEET GECOPY-PASTE
 * DIT KOMT DOORDAT DIT EEN OUDERE .NET TARGET FRAMEWORK IS
 * WAARDOOR WIJ ONZE LIBRARY NIET ALS REFERENCE KUNNEN TOEVOEGEN
 * </summary>
 */
public class Encryption
{
	private readonly static int KeySize = 2048;
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
	 * <returns>(string, string) - De publicKey en privateKey</returns>
	 */
	public static (string, string) GenerateRsaKeyPair()
	{
		using RSA rsa = RSA.Create(KeySize);
		string publicKey = rsa.ToXmlString(false);
		string privateKey = rsa.ToXmlString(true);
		return (publicKey, privateKey);
	}
	
	/**
	 * <summary>
	 * Encrypt de data met RSA en een padding van SHA256
	 * </summary>
	 * <param name="dataToEncrypt">De data die encrypt moet worden</param>
	 * <param name="publicKeyReceiver">De public key van de ontvanger</param>
	 * <returns>byte[] van de encrypted data</returns>
	 */
	public static byte[] EncryptData(byte[] dataToEncrypt, string publicKeyReceiver)
	{
		using RSA rsa = RSA.Create(KeySize);
		rsa.FromXmlString(publicKeyReceiver);
		return rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA256);
	}

	/**
	* <summary>
	* Decrypt de data met RSA en een padding van SHA256
	* </summary>
	* <param name="dataToDecrypt">De data die decrypt moet worden</param>
	* <param name="privateKeyReceiver">De private key van de ontvanger</param>
	* <returns>String met de gedecodeerde data</returns>
	*/
	public static string DecryptData(byte[] dataToDecrypt, string privateKeyReceiver)
	{
		using RSA rsa = RSA.Create(KeySize);
		rsa.FromXmlString(privateKeyReceiver);
		return Encoding.UTF8.GetString((rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.OaepSHA256)));
	}

	public static byte[] HashData(string dataToHash)
	{
		return SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(dataToHash));
	}
}