using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FileEncrypt
{
  public class SymmetricAlgorithmFactory : ISymmetricAlgorithmFactory
  {
    public SymmetricAlgorithm Create(byte[] key, int keySize)
    {
      var aes = Aes.Create();
      aes.KeySize = keySize;
      aes.Key = key;
      aes.Mode = CipherMode.CBC;
      return aes;
    }
  }
}
