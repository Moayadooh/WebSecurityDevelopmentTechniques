using System;
using System.Collections.Generic;
using System.Text;

namespace FileEncrypt
{
  public class Constants
  {
    public const int MinPasswordLength = 20;
    public static int[] ValidLengths = new int[] { 128, 192, 256 };
  }
}
