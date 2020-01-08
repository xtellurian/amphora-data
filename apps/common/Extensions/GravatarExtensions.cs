using System;
using System.Security.Cryptography;
using System.Text;

namespace Amphora.Common.Extensions
{
    public static class GravatarExtensions
    {
        public static string HashEmailForGravatar(string email)
        {
            if (email == null)
            {
                throw new NullReferenceException("Email cannot be null");
            }

            // Create a new Stringbuilder to collect the bytes,
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Create a new instance of the MD5CryptoServiceProvider object.
            using (var md5Hasher = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email.Trim().ToLower()));
                // Loop through each byte of the hashed data
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
            }

            return sBuilder.ToString();  // Return the hexadecimal string.
        }
    }
}