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
      if (password == null)
      {
        throw new ArgumentNullException(nameof(password));
      }
      if (password.Length < Constants.MinPasswordLength)
      {
        throw new ArgumentException(paramName: nameof(password), message: $"Passwords must be at least {Constants.MinPasswordLength} characters long");
      }
      if (!Constants.ValidLengths.Any(k => k == keyLength))
      {
        throw new ArgumentException(paramName: nameof(keyLength), message: $"Invalid key length");
      }

      var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
      pbkdf2.IterationCount = iterations;

      var bytes = pbkdf2.GetBytes(keyLength / 8);
      Debug.Assert(bytes.Length * 8 == keyLength);
      return bytes;
    }
  }
}
