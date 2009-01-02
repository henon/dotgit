using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using dotGit.Objects.Storage;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;


namespace dotGit.Objects
{
  public class Zlib
  {
    private static readonly int bufferLength = 1024;

    public static MemoryStream Decompress(string path)
    {
      byte[] buffer = new byte[bufferLength];
      int size;

      MemoryStream output = new MemoryStream();
      using (FileStream fs = new FileStream(path, System.IO.FileMode.Open))
      {
        using (InflaterInputStream inflaterStream = new InflaterInputStream(fs))
        {
          while ((size = inflaterStream.Read(buffer, 0, buffer.Length)) > 0 )
              output.Write(buffer, 0, size);
        }
      }
      return output;
    }

    public static byte[] Decompress(byte[] input)
    {
      byte[] buffer = new byte[bufferLength];
      int size;

      using (MemoryStream output = new MemoryStream(input))
      {
        using (InflaterInputStream inflaterStream = new InflaterInputStream(output))
        {
          while ((size = inflaterStream.Read(buffer, 0, buffer.Length)) > 0)
            output.Write(buffer, 0, size);
        }

        return output.ToArray();
      }
    }

    public static MemoryStream Decompress(GitObjectReader input, long destLength)
    {

      MemoryStream output = new MemoryStream();
      Inflater inflater = new Inflater();

      byte[] buffer = new byte[destLength];

      while (output.Length < destLength)
      {
        if (inflater.IsNeedingInput)
          inflater.SetInput(input.ReadBytes(bufferLength));

        int outLength = inflater.Inflate(buffer);

        if (outLength > 0)
          output.Write(buffer, 0, outLength);
        else
        {
          if (inflater.IsFinished)
            break;
        }
      }

      input.Position -= inflater.RemainingInput;


      return output;
    }

  }
}