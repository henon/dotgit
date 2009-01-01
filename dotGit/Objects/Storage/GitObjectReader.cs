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
    /// <returns>byte[]</returns>
    public byte[] ReadToChar(char stop, bool consume)
    {
      using (MemoryStream ms = new MemoryStream())
      {
        byte buffer;

        while ((buffer = ReadByte()) != stop)
        {
          ms.WriteByte(buffer);
        }

        if (!consume)
          BaseStream.Position--;

        return ms.ToArray();
      }
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
    public byte[] ReadWord()
    {
      return ReadToChar(' ', true);
    }

    public byte[] ReadWord(bool consumeSpace)
    {
      return ReadToChar(' ', consumeSpace);
    }

    /// <summary>
    /// Reads bytes from the stream until a newline character (\n) is found. The newline is skipped.
    /// </summary>
    /// <returns></returns>
    public byte[] ReadLine()
    {
      return ReadToChar('\n', true);
    }

    public byte[] ReadToNull()
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

      string typeString = ReadWord().GetString();
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
