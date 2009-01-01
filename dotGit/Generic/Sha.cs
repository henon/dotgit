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
    private static readonly SHA1 _sha = SHA1.Create();
    private int[] words = new int[5];

    public static string Compute(byte[] contents)
    {
      byte[] computedHash = _sha.ComputeHash(contents);
      return Decode(computedHash);
    }

    public static string Decode(byte[] sha)
    {
      return BitConverter.ToString(sha).Replace("-", "").ToLower();
    }

    internal static string Compute(GitObjectReader input)
    {
      long oldPosition = input.BaseStream.Position;

      input.Rewind();

      string hash = Compute(input.ReadToEnd());

      input.BaseStream.Position = oldPosition;

      return hash;
    }

    #region Constructor

    public Sha(string sha)
    {
			SHAString = sha;

      byte[] b = HexToData(sha);
     
      words = new int[5];

      FirstByte = (int)b[0];

      words[0] = System.Net.IPAddress.HostToNetworkOrder(System.BitConverter.ToInt32(b, 0));
      words[1] = System.Net.IPAddress.HostToNetworkOrder(System.BitConverter.ToInt32(b, 4));
      words[2] = System.Net.IPAddress.HostToNetworkOrder(System.BitConverter.ToInt32(b, 8));
      words[3] = System.Net.IPAddress.HostToNetworkOrder(System.BitConverter.ToInt32(b, 12));
      words[4] = System.Net.IPAddress.HostToNetworkOrder(System.BitConverter.ToInt32(b, 16));

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

    public override string ToString()
		{
			return SHAString;
		}

    internal int CompareTo(int[] data, int idx)
    {

      int result;

      result = words[0].CompareTo(data[idx]);
      if (result != 0)
        return result;

      result = words[1].CompareTo(data[idx + 1]);
      if (result != 0)
        return result;

      result = words[2].CompareTo(data[idx + 2]);
      if (result != 0)
        return result;

      result = words[3].CompareTo(data[idx + 3]);
      if (result != 0)
        return result;

      return words[4].CompareTo(data[idx + 4]);
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

    #endregion
  }
}