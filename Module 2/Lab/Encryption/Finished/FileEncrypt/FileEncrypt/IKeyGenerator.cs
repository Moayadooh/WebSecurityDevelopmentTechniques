namespace FileEncrypt
{
  public interface IKeyGenerator
  {
    byte[] GenerateKeyFromPassword(string password, int keyLength, byte[] salt, int iterations = 10000);
  }
}