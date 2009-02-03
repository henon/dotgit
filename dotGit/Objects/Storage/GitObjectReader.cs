using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using dotGit.Objects.Storage;
using dotGit.Exceptions;

namespace dotGit.Objects.Storage
{
  /// <summary>
  /// The GitObjectReader class is used by the dotGit library as a wrapper around several lower level file/memory streams.
  /// </summary>
  public class GitObjectReader : BinaryReader
  {
    public GitObjectReader(Stream stream)
      : base(stream, Encoding.ASCII)
    { }

    public GitObjectReader(byte[] contents)
      : this(new MemoryStream(contents))
    { }

    public byte[] ReadToNextNonNull()
    {
      return SkipChars('\0');
    }

    /// <summary>
    /// Read bytes from the stream while the character is <param>charToSkip</param>. Returns the read characters as a byte-array.
    /// </summary>
    /// <param name="charToSkip">Character to skip</param>
    public byte[] SkipChars(char charToSkip)
    {
      using (MemoryStream ms = new MemoryStream())
      {
        while (PeekChar() == charToSkip)
        {
          ms.WriteByte(ReadByte());
        }

        return ms.ToArray();
      }
    }

    /// <summary>
    /// Read bytes from the stream until the stop character is found. The <paramref name="consume"/> parameter controls wheter the stream position 
    /// needs to be reset after reading from the stream
    /// </summary>
    /// <param name="stop">Character to stop reading at</param>
    /// <param name="consume">boolean value indicating if we want to reset the position in the stream after reading</param>
    /// <returns>string</returns>
    public string ReadToChar(char stop, bool consume)
    {

      // [Skinny] This version of ReadToChar actually performs better as reading one byte at a time but needs more testing

      long position = BaseStream.Position;
      int bufLength = 4;

      StringBuilder sb = new StringBuilder();
      byte[] b = new byte[bufLength];
      bool found = false;
      int index = -1;

      while (!found)
      {
        b = ReadBytes(bufLength);
        for (int idx = 0; idx < bufLength; idx++)
        {
          if (b[idx] == (byte)stop)
          {
            index = idx;
            found = true;
            break;
          }
          else
          {
            sb.Append((char)b[idx]);
          }
        }
      }


      BaseStream.Position -= ( (bufLength - index) - (consume ? 1 : 0 ));

      return sb.ToString();

    }

    /// <summary>
    /// Reads from the stream until the bytes read can be inflated to the size passed in the <paramref name="destLength"/> parameter.
    /// </summary>
    /// <param name="destLength">Desired uncompressed object size</param>
    /// <returns>MemorySream containing the inflated contents</returns>
    public MemoryStream UncompressToLength(long destLength)
    {
      return Zlib.Decompress(this, destLength);
    }

    /// <summary>
    /// Reads bytes from the stream until a space character is found. The bytes read are returned from this function.
    /// </summary>
    /// <returns></returns>
    public string ReadWord()
    {
      return ReadToChar(' ', true);
    }

    /// <summary>
    /// Reads bytes from the stream until a newline character (\n) is found. The newline is skipped.
    /// </summary>
    /// <returns></returns>
    public string ReadLine()
    {
      return ReadToChar('\n', true);
    }

    public string ReadToNull()
    {
      return ReadToChar('\0', true);
    }

    public void Rewind()
    {
      BaseStream.Position = 0;
    }

    public byte[] ReadToEnd()
    {
      return ReadBytes((int)(BaseStream.Length - Position));
    }

    public long ReadObjectHeader(out ObjectType type)
    {
      if (!IsStartOfStream)
        Rewind();

      long length = 0;

      string typeString = ReadWord();
      switch (typeString)
      {
        case "blob":
          type = ObjectType.Blob;
          break;
        case "commit":
          type = ObjectType.Commit;
          break;
        case "tag":
          type = ObjectType.Tag;
          break;
        case "tree":
          type = ObjectType.Tree;
          break;
        default:
          throw new ParseException("Unknown type: {0}".FormatWith(typeString));
      }

      byte buffer = ReadByte();
      int bitCount = 8;
      while (buffer != '\0')
      {
        length |= (long)buffer << bitCount;
        bitCount += 8;
        buffer = ReadByte();
      }

      return length;
    }

    public string GetString(int numberOfBytes)
    {
      return ReadBytes(numberOfBytes).GetString();
    }

    public bool IsEndOfStream
    {
      get { return BaseStream.Position >= BaseStream.Length; }
    }

    public bool IsStartOfStream
    {
      get { return BaseStream.Position == 0; }
    }

    public long Position
    {
      get { return BaseStream.Position; }
      set { BaseStream.Position = value; }
    }
  }
}
