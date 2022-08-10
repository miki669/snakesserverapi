using System.Security.Cryptography;
using System.Text;

namespace SnakeServerAPI.Encrypt
{
    public class Operation
    {
        public static string Decrypt(string PrivateKey, string cipher)
        {
            using RSACryptoServiceProvider RSA2048 = new(2048);
            RSA2048.PersistKeyInCsp = false;
            RSA2048.FromXmlString(PrivateKey);
            try
            {
            return Encoding.UTF8.GetString(RSA2048.Decrypt(Convert.FromBase64String(cipher), false));
            }
            catch { return "HYU"; }
            
        }
    }
}
