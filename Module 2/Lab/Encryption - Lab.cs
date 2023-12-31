/*Introduction	
In this lab you will build a command line tool to encrypt and decrypt files using the AES symmetric encryption algorithm.
This lab starts from starter files which you can download from online.u2u.be. At the end of the lab the full solution if provided. Please try to refrain from using the solution, it will make the exercise more interesting for you.
Examining the starter files
Open the starter file’s solution (.sln). Open program.cs. The Main method creates a couple of dependencies and then uses the FileCryptor class to either encrypt or decrypt a file. Main uses a NuGet package called CommandLineParser to parse the command line arguments and picks the right method to call.*/
//PROGRAM.CS
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
 
      return CommandLine.Parser
                        .Default
                        .ParseArguments<EncryptOptions, DecryptOptions>(args)
        .MapResult(
        (EncryptOptions options) => fc.EncryptFiles(options),
        (DecryptOptions options) => fc.DecryptFiles(options),
        errors => 1
        );
    }
  }
}
//The CommandLine.Parser uses two Options derived classes to decide what the command line arguments should be:
//OPTIONS.CS
using CommandLine;
using System;
using System.Linq;
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
//Your mission is to implement the dependencies used by this program.

/*Implement SaltGenerator
Start by examing the ISaltGenerator interface. This interface has only one method which should return a random salt of a certain length (keyLength).*/

//ISALTGENERATOR.CS
namespace FileEncrypt
{
  public interface ISaltGenerator
  {
    byte[] GenerateSalt(int keyLength);
  }
}
//Implement the SaltGenerator class. Use the RandomNumberGenerator class for this which will generate a cryptographically random sequence of bytes.

/*Implement KeyGenerator
Look at the IKeyGenerator interface. This class will generate a key from a password.*/

//IKEYGENERATOR.CS
namespace FileEncrypt
{
  public interface IKeyGenerator
  {
    byte[] GenerateKeyFromPassword(string password, int keyLength,
                                   byte[] salt, int iterations = 10000);
  }
}
//Implement the KeyGenerator class using the Rfc2898DeriveBytes class.

/*Implement FileCryptor
Examine the FileCryptor class, start with the EncryptFiles method. Your task is to open the options.OutputFile and write the salt, initialization vector and options.InputFile. Both salt and IV don’t need to be encrypted, but you will have to encrypt the options.InputFile. For this use the CryptoStream class.

Test your implementation by setting the Application Arguments in the project’s Properties window.

For example:*/

encrypt -o File.cpr -p IDontWannaDoThisBrother -f File.txt
//Next mission is to implement the DecryptFiles method in a similar fashion (but of course decrypt the input file).

/*Solution
The solution is provided here in case you get stuck.*/

//SALTGENERATOR.CS
public class SaltGenerator : ISaltGenerator
{
  public byte[] GenerateSalt(int keyLength)
  {
    byte[] salt = new byte[keyLength];
    RandomNumberGenerator.Create().GetBytes(salt);
    return salt;
  }
}
//KEYGENERATOR.CS
public class KeyGenerator : IKeyGenerator
{
  public byte[] GenerateKeyFromPassword(string password, int keyLength,
                                        byte[] salt, int iterations = 10000)
  {
    if (password == null)
    {
      throw new ArgumentNullException(nameof(password));
    }
    if (password.Length < Constants.MinPasswordLength)
    {
      throw new ArgumentException(
        paramName: nameof(password), 
        message: $"Passwords must be at least {Constants.MinPasswordLength} characters long");
    }
    if (!Constants.ValidLengths.Any(k => k == keyLength))
    {
      throw new ArgumentException(
        paramName: nameof(keyLength), 
        message: $"Invalid key length");
    }
 
    var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
    pbkdf2.IterationCount = iterations;
 
    var bytes = pbkdf2.GetBytes(keyLength / 8);
    Debug.Assert(bytes.Length * 8 == keyLength);
    return bytes;
  }
}
//FILECRYPTOR.CS
public class FileCryptor
{
  private ISaltGenerator saltGen;
  private IKeyGenerator keyGen;
  private ISymmetricAlgorithmFactory aesFactory;
 
  public FileCryptor(ISaltGenerator saltGen, IKeyGenerator keyGen, 
                     ISymmetricAlgorithmFactory aesFactory)
  {
    this.saltGen = saltGen ?? 
                   throw new ArgumentNullException(nameof(saltGen));
    this.keyGen = keyGen ?? 
                  throw new ArgumentNullException(nameof(keyGen));
    this.aesFactory = aesFactory ?? 
                      throw new ArgumentNullException(nameof(keyGen));
  }
 
  public int EncryptFiles(EncryptOptions options)
  {
    if (options.Valid())
    {
      var salt = saltGen.GenerateSalt(options.KeyLength);
      byte[] key = keyGen.GenerateKeyFromPassword(options.Password, 
                     options.KeyLength, salt, options.Iterations);
 
      SymmetricAlgorithm aes = aesFactory.Create(key, options.KeyLength);
      ICryptoTransform enc = aes.CreateEncryptor();
 
      using (Stream output = File.OpenWrite(options.OutputFile))
      {
        output.Write(salt, offset: 0, count: options.KeyLength);
        output.Write(aes.IV, offset: 0, count: aes.IV.Length);
 
        using (var cs = new CryptoStream(output, enc, CryptoStreamMode.Write))
        {
          Stream s = File.OpenRead(options.InputFile);
          byte[] buf = new byte[2048];
          int read = 0;
          while ((read = s.Read(buf, 0, buf.Length)) > 0)
          {
            cs.Write(buf, offset: 0, count: read);
          }
        }
      }
      return 0;
    }
    else
    {
      return -1;
    }
  }
 
  public int DecryptFiles(DecryptOptions options)
  {
    if (options.Valid())
    {
      Stream input = File.OpenRead(options.InputFile);
      byte[] salt = new byte[options.KeyLength];
      input.Read(salt, offset: 0, count: options.KeyLength);
 
      var keyGen = new KeyGenerator();
      byte[] key = keyGen.GenerateKeyFromPassword(options.Password, 
                     options.KeyLength, salt, options.Iterations);
 
      SymmetricAlgorithm aes = aesFactory.Create(key, options.KeyLength);
 
      byte[] IV = new byte[aes.IV.Length];
      input.Read(IV, 0, IV.Length);
      aes.IV = IV;
 
      ICryptoTransform enc = aes.CreateDecryptor();
      using (var cs = new CryptoStream(input, enc, CryptoStreamMode.Read))
      using (var output = File.OpenWrite(options.OutputFile))
      {
        byte[] buf = new byte[2048];
        int read = 0;
        while ((read = cs.Read(buf, 0, buf.Length)) > 0)
        {
          output.Write(buf, offset: 0, count: read);
        }
      }
      return 0;
    }
    else
    {
      return -1;
    }
  }
}