using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FileEncrypt
{
  public interface ISymmetricAlgorithmFactory
  {
    SymmetricAlgorithm Create(byte[] key, int keySize);
  }
}
