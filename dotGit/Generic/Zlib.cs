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
    public static MemoryStream Decompress(string path)
    {
      return Decompress(new GitObjectReader(File.OpenRead(path)));
    }

    public static MemoryStream Decompress(BinaryReader input)
    {
      var output = new MemoryStream();
      var zipStream = new Inflater();

      using (input)
      {
        var buffer = new byte[2000];
        int len;

        while (input.BaseStream.Position < input.BaseStream.Length)
        {
          if (zipStream.IsNeedingInput)
            zipStream.SetInput(input.ReadBytes(buffer.Length));

          zipStream.Inflate(buffer);
          output.Write(buffer, 0, buffer.Length);
        }
      }

      output.Position = 0;

      byte[] content = new byte[output.Length];

      output.Read(content, 0, (int)output.Length);

      return output;
    }

    public static byte[] Decompress(byte[] input)
    {
      using (MemoryStream output = new MemoryStream())
      {
        using (InflaterInputStream zipStream = new InflaterInputStream(output))
        {
          using (MemoryStream inputStream = new MemoryStream(input))
          {
            var buffer = new byte[2000];
            int len;

            while ((len = inputStream.Read(buffer, 0, 2000)) > 0)
            {
              zipStream.Write(buffer, 0, len);
            }
          }
        }
        return output.ToArray();
      }
    }

    public static MemoryStream Decompress(GitObjectReader input, long destLength)
    {
      int bufferLength = 1024;


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