using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotGit.Objects.Storage
{
  /// <summary>
  /// Represents a Pack Index sorted on by Sha values to support quick retrieval of a specific Sha offset.  
  /// </summary>
  internal class PackReverseIndexV2
  {
    private int[] offsets; 
    private int[] objectList; 

    internal PackIndexV2 Index
    {
      get;
      set;
    }
    
    internal PackReverseIndexV2(PackIndexV2 index)
    {
      Index = index;

      // Traverse index and build reverse-version
      offsets = new int[Index.NumberOfObjects];
      objectList = new int[Index.NumberOfObjects];


      int indexCount = 0;
      foreach (PackIndexEntry entry in Index)
      {
        offsets[indexCount] = entry.Offset;
        indexCount++;
      }

      Array.Sort(offsets);

      indexCount = 0;
      foreach (PackIndexEntry entry in Index)
      {
        objectList[Array.BinarySearch(offsets, entry.Offset)] = indexCount;
        indexCount++;
      }
    }

    internal PackIndexEntry GetObject(int offset)
    {
      int index = Array.BinarySearch(offsets, offset);
      if (index < 0)
        return null;

      return Index.GetEntry(objectList[index]);
    }

    internal int GetNextOffset(int offset, int maxOffset)
    {
      int index = Array.BinarySearch(offsets, offset);

      return offsets[index + 1];
    }
  }
}
