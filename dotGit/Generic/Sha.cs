using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using dotGit.Objects;
using dotGit.Objects.Storage;

namespace dotGit.Generic
{
  /// <summary>
  /// Class representing a SHA 'object' in the Git repository. It also has a couple of helper functions to make comparing and
  /// converting from/to SHA's easier.
  /// </summary>
  public class Sha
  {
    public static readonly int ShaByteLength = 20;
    public static readonly int ShaStringLength = 40;

    #region Fields

    private static readonly char[] hexStringCharacters = { '0', '1', '2', '3', '4', '5', '6',
			'7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

    private int[] _words = new int[5];

    #endregion

    #region Public Methods

    public static string Decode(byte[] sha)
    {
      return BitConverter.ToString(sha).Replace("-", "").ToLower();
    }

    public static bool IsValidSha(string sha)
    {
      return Utility.SHAExpression.IsMatch(sha);
    }
    #endregion

    #region Constructors

    public Sha(string sha)
    {
      SHAString = sha;

      _words = new int[5];

      byte[] b = HexToData(sha);

      FirstByte = (int)b[0];

      _words[0] = System.Net.IPAddress.HostToNetworkOrder(System.BitConverter.ToInt32(b, 0));
      _words[1] = System.Net.IPAddress.HostToNetworkOrder(System.BitConverter.ToInt32(b, 4));
      _words[2] = System.Net.IPAddress.HostToNetworkOrder(System.BitConverter.ToInt32(b, 8));
      _words[3] = System.Net.IPAddress.HostToNetworkOrder(System.BitConverter.ToInt32(b, 12));
      _words[4] = System.Net.IPAddress.HostToNetworkOrder(System.BitConverter.ToInt32(b, 16));

    }

    public Sha(int[] words)
    {
      this._words = words;
      SHAString = MakeHexString();

      FirstByte = BitConverter.GetBytes(System.Net.IPAddress.NetworkToHostOrder(_words[0]))[0];
    }

    public Sha(int[] input, int position)
    {
      _words[0] = input[position];
      _words[1] = input[position + 1];
      _words[2] = input[position + 2];
      _words[3] = input[position + 3];
      _words[4] = input[position + 4];

      SHAString = MakeHexString();

      FirstByte = BitConverter.GetBytes(System.Net.IPAddress.NetworkToHostOrder(_words[0]))[0];
    }

    #endregion

    #region Properties

    public int FirstByte
    {
      get;
      private set;
    }

    public string SHAString
    {
      get;
      private set;
    }

    #endregion

    #region System Overrides

    public override int GetHashCode()
    {
      return SHAString.GetHashCode();
    }
    public override string ToString()
    {
      return SHAString;
    }

    internal int CompareTo(int[] data, int idx)
    {

      int result;

      result = _words[0].CompareTo(data[idx]);
      if (result != 0)
        return result;

      result = _words[1].CompareTo(data[idx + 1]);
      if (result != 0)
        return result;

      result = _words[2].CompareTo(data[idx + 2]);
      if (result != 0)
        return result;

      result = _words[3].CompareTo(data[idx + 3]);
      if (result != 0)
        return result;

      return _words[4].CompareTo(data[idx + 4]);
    }

    public override bool Equals(object obj)
    {
      if (obj is Sha)
        return this.SHAString.Equals(((Sha)obj).SHAString);
      else
        return false;
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Function to convert a hex-string SHA to a byte array. Thanks to George Helyar (http://www.codeproject.com/KB/recipes/hexencoding.aspx?fid=15401&df=90&mpp=25&noise=3&sort=Position&view=Quick&select=2494780#xx2494780xx)
    /// </summary>
    private byte[] HexToData(string hexString)
    {
      if (hexString == null)
        return null;

      if (hexString.Length % 2 == 1)
        hexString = '0' + hexString;

      byte[] data = new byte[hexString.Length / 2];

      for (int i = 0; i < data.Length; i++)
        data[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

      return data;
    }


    /// <summary>
    /// Returns a string representation of a Sha stored in _words. This should be faster as the default .NET String convert functions.
    /// </summary>
    /// <returns>string</returns>
    private string MakeHexString()
    {

      char[] b = new char[ShaStringLength];

      formatHexChar(ref b, 0, _words[0]);
      formatHexChar(ref b, 8, _words[1]);
      formatHexChar(ref b, 16, _words[2]);
      formatHexChar(ref b, 24, _words[3]);
      formatHexChar(ref b, 32, _words[4]);

      return new string(b);

    }

    private static void formatHexChar(ref char[] dst, int p, int w)
    {
      int o = p + 7;
      while (o >= p && w != 0)
      {
        dst[o--] = hexStringCharacters[w & 0xf];
        w >>= 4;
      }
      while (o >= p)
        dst[o--] = '0';
    }


    #endregion

 
  }
}