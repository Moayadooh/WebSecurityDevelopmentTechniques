using CommandLine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace FileEncrypt
{
  public class Options
  {
    [Option(shortName: 'f', longName: "file", Default = "*.*", HelpText = "Input file to process")]
    public string InputFile { get; set; }

    [Option(shortName: 'o', longName: "output", Required = true, HelpText = "Output file")]
    public string OutputFile { get; set; }

    [Option(shortName: 'p', longName: "password", Required = true, HelpText = "The password to encrypt/decrypt.")]
    public string Password { get; set; }

    [Option(shortName: 'l', longName: "keylength", Default = 192, HelpText = "Length of key in bits")]
    public int KeyLength { get; set; }

    [Option(shortName: 'i', longName: "iterations", Default = 10000, HelpText = "Nr. of iterations")]
    public int Iterations { get; set; }

    public bool Valid()
    {
      bool valid = true;
      if (Password.Length < Constants.MinPasswordLength)
      {
        valid = false;
        Console.WriteLine($"Password is too short. It should be at least {Constants.MinPasswordLength} characters long.");
      }
      if (!Constants.ValidLengths.Contains(KeyLength))
      {
        valid = false;
        Console.WriteLine($"Key length is invalid. Choose from {Constants.ValidLengths}.");
      }
      if(!File.Exists(InputFile))
      {
        valid = false;
        Console.WriteLine($"File {InputFile} not found.");
      }
      return valid;
    }
  }

  [Verb(name: "encrypt", HelpText = "Encrypt files", Hidden = false)]
  public class EncryptOptions : Options
  {
  }

  [Verb(name: "decrypt", HelpText = "Decrypt files", Hidden = false)]
  public class DecryptOptions : Options
  {
  }

}
