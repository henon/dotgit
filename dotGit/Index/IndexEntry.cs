using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotGit.Generic;
using dotGit.Objects;
using System.IO;
using dotGit.Objects.Storage;

namespace dotGit.Index
{
	public class IndexEntry
	{
		internal IndexEntry(GitObjectReader source)
		{
      long startPosition = source.Position;

      Created = GetFileTime(source);
      Modified = GetFileTime(source);

      // TODO: really parse all the var stuff
      var dev = System.Net.IPAddress.HostToNetworkOrder(source.ReadBytes(4).ToInt());
			var ino = System.Net.IPAddress.HostToNetworkOrder(source.ReadBytes(4).ToInt());
			var mode = System.Net.IPAddress.HostToNetworkOrder(source.ReadBytes(4).ToInt());
			var uid = System.Net.IPAddress.HostToNetworkOrder(source.ReadBytes(4).ToInt());
      var gid = System.Net.IPAddress.HostToNetworkOrder(source.ReadBytes(4).ToInt());

      Size = System.Net.IPAddress.HostToNetworkOrder(source.ReadBytes(4).ToInt());
			SHA = source.ReadBytes(20).ToSHAString();

			var flags = source.ReadBytes(2);
			var assumeValid = flags[0].GetBits(0, 1);
			var updateNeeded = flags[0].GetBits(1, 1);
			Stage = (IndexStage)flags[0].GetBits(2, 2);
			
			Path = source.ReadToNull();

      // Read bytes until the length of this entry can be divided by 8 and the name (Path) is still char(0) terminated
      // http://kerneltrap.org/index.php?q=mailarchive/git/2008/2/11/810634

      long endPosition = source.Position;
      long length = (endPosition - startPosition) ;
      if( length %8 != 0)
        source.ReadBytes(8 - ((int)length % 8));

    }

    private DateTime GetFileTime(GitObjectReader source)
    {
      int seconds = System.Net.IPAddress.HostToNetworkOrder(source.ReadBytes(4).ToInt());
      int nanoseconds = System.Net.IPAddress.HostToNetworkOrder(source.ReadBytes(4).ToInt());

      return Utility.UnixEPOCH.AddSeconds(seconds).AddMilliseconds(nanoseconds / 1000);
    }

    #region Properties

    public string Path
		{
			get;
			private set;
		}

		public string SHA
		{
			get;
			private set;
		}

		public IndexStage Stage
		{
			get;
			private set;
		}

    public DateTime Created
		{
			get;
			private set;
		}

    public DateTime Modified
		{
			get;
			private set;
		}

		public int Size
		{
			get;
			private set;
    }

    #endregion
  }

	public enum IndexStage
	{
		Normal,
		Ancestor,
		Our,
		Their
	}

}
