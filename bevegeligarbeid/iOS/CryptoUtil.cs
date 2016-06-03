// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES: iOS Implementation of Triona AS' ICryptoUtil
//======================================================
using TheWallClient.PCL.Service;
using System.Security.Cryptography;

namespace BevegeligArbeid.iOS
{
    public class CryptoUtil : ICryptoUtil
	{

		public CryptoUtil (string guid){

		}
	
		private static RijndaelManaged GetAlgorithm (string encryptionPassword){
			return new RijndaelManaged ();
		}

		private static byte[] InMemoryCrypt (byte[] data, ICryptoTransform transform){
			return new byte[] { 0 };
		}

		public string Dectypt(string encryptedMessage, string subject){
			return "Dectypt";
		}

		public string Encrypt(string secretMessage, string subject){
			return "Encrypt";
		}
	}
}

