
namespace Application_Security_Project_v2.Services
{
    public class EncryptionService
    {
        public string Encrypt(string unencryptedData)
        {
            byte[] encData_byte = new byte[unencryptedData.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(unencryptedData);
            string encodedData = Convert.ToBase64String(encData_byte);
            return encodedData;
        }

        public string Decrypt(string encryptedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
    }
}
