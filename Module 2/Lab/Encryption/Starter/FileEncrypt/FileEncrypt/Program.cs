using CommandLine;
using System;
using System.IO;
using System.Security.Cryptography;

namespace FileEncrypt
{
  internal class Program
  {
    private static int Main(string[] args)
    {
      // Create dependencies...
      ISaltGenerator saltGen = new SaltGenerator();
      IKeyGenerator keyGen = new KeyGenerator();
      ISymmetricAlgorithmFactory aesFactory = new SymmetricAlgorithmFactory();
      FileCryptor fc = new FileCryptor(saltGen, keyGen, aesFactory);

      return CommandLine.Parser.Default.ParseArguments<EncryptOptions, DecryptOptions>(args)
        .MapResult(
        (EncryptOptions options) => fc.EncryptFiles(options),
        (DecryptOptions options) => fc.DecryptFiles(options),
        errors => 1
        );
    }
  }
}
