using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FileEncrypt
{
    public class KeyGenerator : IKeyGenerator
    {
        public byte[] GenerateKeyFromPassword(string password, int keyLength, byte[] salt, int iterations = 10000)
        {
            return Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA1, keyLength);
        }
    }
}
