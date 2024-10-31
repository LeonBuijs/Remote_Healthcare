using System.Security.Cryptography;
using System.Text;

namespace SecurityManager;

/**
 * <summary>
 * Klasse die alle encryptie en decryptie afhandelt van data
 * Kan ook een keypair genereren
 * </summary>
 */
public class Encryption
{
	/**
	 * <summary>
	 * Genereert een publicKey en privateKey
	 * Vervolgens zet hij die in out parameters in de vorm van een string
	 * </summary>
	 * <param name="publicKey">De out string value van de public key</param>
	 * <param name="privateKey">De out string value van de private key</param>
	 */
	public static void GenerateRsaKeyPair(out string publicKey, out string privateKey)
	{
		using RSA rsa = RSA.Create(1024);
		publicKey = rsa.ExportRSAPublicKeyPem();
		privateKey = rsa.ExportRSAPrivateKeyPem();
	}	

	/**
	 * <summary>
	 * Encrypt de data met RSA en een padding van SHA256
	 * </summary>
	 * <param name="dataToEncrypt">De data die encrypt moet worden</param>
	 * <param name="publicKeyReceiver">De public key van de ontvanger</param>
	 * <returns>byte[] van de encrypted data</returns>
	 */
	public static byte[] EncryptData(string dataToEncrypt, string publicKeyReceiver)
	{
		byte[] dataToEncryptBytes = Encoding.ASCII.GetBytes(dataToEncrypt);
		using RSA rsa = RSA.Create();
		rsa.ImportFromPem(publicKeyReceiver);
		return rsa.Encrypt(dataToEncryptBytes, RSAEncryptionPadding.OaepSHA256);
	}

	/**
	* <summary>
	* Decrypt de data met RSA en een padding van SHA256
	* </summary>
	* <param name="dataToDecrypt">De data die decrypt moet worden</param>
	* <param name="privateKeyReceiver">De private key van de ontvanger</param>
	* <returns>byte[] van de decrypted data</returns>
	*/
	public static byte[] DecryptData(string dataToDecrypt, string privateKeyReceiver)
	{
		byte[] dataToDecryptBytes = Convert.FromBase64String(dataToDecrypt);
		using RSA rsa = RSA.Create();
		rsa.ImportFromPem(privateKeyReceiver);
		return rsa.Decrypt(dataToDecryptBytes, RSAEncryptionPadding.OaepSHA256);
	}

	public static byte[] HashData(string dataToHash)
	{
		return SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(dataToHash));
	}
}