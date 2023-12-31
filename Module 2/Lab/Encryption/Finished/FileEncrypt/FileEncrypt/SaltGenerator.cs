using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FileEncrypt
{
  public class SaltGenerator : ISaltGenerator
  {
    public byte[] GenerateSalt(int keyLength)
    {
      byte[] salt = new byte[keyLength];
      RandomNumberGenerator.Create().GetBytes(salt);
      return salt;
    }
    
  }
}
