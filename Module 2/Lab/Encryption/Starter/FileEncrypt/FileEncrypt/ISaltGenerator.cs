namespace FileEncrypt
{
  public interface ISaltGenerator
  {
    byte[] GenerateSalt(int keyLength);
  }
}