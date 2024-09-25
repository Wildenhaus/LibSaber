using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSaber.IO;
using static LibSaber.SpaceMarine2.Structures.Resources.fioZIP_CACHE_FILE;

namespace Testbed;

internal class SDC
{

  public Dictionary<uint, string>  CrcMap { get; }
  public SDCHeader Header { get; private set; }
  public List<SDCGroup> Groups { get; private set; }
  public byte[] BulkData { get; private set; }

  public SDC(NativeReader reader, Dictionary<uint, string> crcMap)
  {
    CrcMap = crcMap;
    Header = new SDCHeader(reader);
    Groups = new List<SDCGroup>();

    for (int i = 0; i < Header.GroupCount; i++)
    {
      var group = new SDCGroup(reader);
      Groups.Add(group);
    }

    var data = new byte[Header.DataSize];
    reader.Read(data);
    BulkData = data;

    foreach(var group in Groups)
    {
      foreach(var entry in group.Entries)
      {
        if (crcMap.ContainsKey(entry.NameCRC))
        {
          var name = crcMap[entry.NameCRC];
          entry.Name = name;
          //Debugger.Break();
        }
      }
    }
  }

  public void Extract(string filename)
  {
    File.WriteAllBytes(filename, BulkData);
  }

  public void ExtractAll(string outDir)
  {
    int offset = 0;

    for (int i = 0; i < Groups.Count; i++)
    {
      var group = Groups[i];
      byte[] zlibData = BulkData.Skip(offset).Take((int)group.DataSize).ToArray();
      offset += (int)group.DataSize;

      byte[] decompressedData;
      using (var compressedStream = new MemoryStream(zlibData))
      using (var deflateStream = new ZLibStream(compressedStream, CompressionMode.Decompress))
      using (var decompressedStream = new MemoryStream())
      {
        deflateStream.CopyTo(decompressedStream);
        decompressedData = decompressedStream.ToArray();
      }

      int dataOffset = 0;
      foreach (var entry in group.Entries)
      {
        var fn = $"{i}_{entry.NameCRC}.bin";
        if (!string.IsNullOrEmpty(entry.Name))
          fn = $"{i}_{entry.Name.Replace("/", "_").Replace("\\", "_")}.bin";
        else
          continue;

        string fileName = fn;
        fileName = Path.Combine(outDir, fileName);

        if (File.Exists(fileName))
          continue;

        byte[] entryData = decompressedData.Skip(dataOffset).Take((int)entry.DataSize).ToArray();
        File.WriteAllBytes(fileName, entryData);
        Console.WriteLine("\tWrote {0}", Path.GetFileName(fileName));
        dataOffset += (int)entry.DataSize;
      }
    }
  }

}

public class SDCHeader
{
  public uint GroupCount { get; private set; }
  public uint EntryCount { get; private set; }
  public uint DataSize { get; private set; }
  public uint Unk { get; private set; }

  public SDCHeader(NativeReader reader)
  {
    GroupCount = reader.ReadUInt32();
    EntryCount = reader.ReadUInt32();
    DataSize = reader.ReadUInt32();
    Unk = reader.ReadUInt32();
  }
}

public class SDCEntry
{
  public uint NameCRC { get; private set; }
  public uint DataSize { get; private set; }
  public string Unk_01 { get; set; }
  public string Name { get; set; }

  public SDCEntry(NativeReader reader)
  {
    NameCRC = reader.ReadUInt32();
    DataSize = reader.ReadUInt32();
    Unk_01 = reader.ReadLengthPrefixedString32();

    //if (!string.IsNullOrEmpty(Unk_01))
    //  Debugger.Break();
  }
}

public class SDCGroup
{
  public uint DataSize { get; private set; }
  public uint EntryCount { get; private set; }
  public List<SDCEntry> Entries { get; private set; }

  public SDCGroup(NativeReader reader)
  {
    DataSize = reader.ReadUInt32();
    EntryCount = reader.ReadUInt32();
    Entries = new List<SDCEntry>();

    for (int i = 0; i < EntryCount; i++)
    {
      var entry = new SDCEntry(reader);
      Entries.Add(entry);
    }
  }
}