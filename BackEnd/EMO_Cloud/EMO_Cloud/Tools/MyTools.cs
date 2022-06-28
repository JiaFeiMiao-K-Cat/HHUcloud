using System.Security.Cryptography;
using System.Text;

namespace EMO_Cloud.Tools
{
    public class MyTools
    {
        public static string ComputeSHA256Hash(string text)
        {
            using (var sha256 = SHA256.Create())
            {
                return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "");
            }
        }
    }
}
