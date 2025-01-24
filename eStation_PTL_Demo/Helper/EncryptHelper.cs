using Serilog;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace eStation_PTL_Demo.Helper
{
    /// <summary>
    /// Encrypt helper
    /// </summary>
    internal class EncryptHelper
    {
        static readonly Aes A;
        static readonly ICryptoTransform E;
        static readonly ICryptoTransform D;
        static readonly byte[] K = [0x7D, 0x56, 0x51, 0x5B, 0x69, 0xE1, 0x66, 0x81, 0x7C, 0x76, 0x2A, 0x35, 0x5D, 0x1D, 0x32, 0x52, 0x1B, 0x41, 0xBD, 0xD9, 0xCB, 0xB8, 0x40, 0x1C, 0xD3, 0x02, 0xDB, 0x36, 0xB8, 0x17, 0x99, 0x67];
        static readonly byte[] I = [0x8F, 0x51, 0xAF, 0x06, 0x0C, 0x66, 0xCD, 0x2D, 0xF6, 0xDC, 0xD1, 0xB6, 0x83, 0x81, 0xD5, 0x9C];

        /// <summary>
        /// Default constructor
        /// </summary>
        static EncryptHelper()
        {
            A = Aes.Create();
            A.Key = K;
            A.IV = I;

            E = A.CreateEncryptor(A.Key, A.IV);
            D = A.CreateDecryptor(A.Key, A.IV);
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <typeparam name="T">Data object type</typeparam>
        /// <param name="t">Data object entity</param>
        /// <returns>Encrypted data</returns>
        public static byte[] Encrypt<T>(T t)
        {
            // Create the streams used for encryption.
            using MemoryStream memory = new();
            using CryptoStream crypto = new(memory, E, CryptoStreamMode.Write);
            using (StreamWriter writer = new(crypto, Encoding.UTF8))
            {
                //Write all data to the stream.
                writer.Write(JsonSerializer.Serialize(t));
            }
            return memory.ToArray();
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="data">Encrypted data array</param>
        /// <returns>(Index, decrypted data)</returns>
        public static string Decrypt(byte[] data)
        {
            try
            {
                using MemoryStream memory = new(data);
                using CryptoStream crypto = new(memory, D, CryptoStreamMode.Read);
                using StreamReader reader = new(crypto, Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Decrypt_Error");
                return string.Empty;
            }
        }
    }
}
