using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSaber.IO;
using LibSaber.SpaceMarine2.Serialization;

namespace LibSaber.SpaceMarine2.Structures.Resources;

public class fioZIP_FILE : IDisposable
{

  private readonly MemoryMappedFile _file;
  private readonly fioZIP_CACHE_FILE _cacheFile;

  private bool _isDisposed;

  public IReadOnlyDictionary<string, fioZIP_CACHE_FILE.ENTRY> Entries => _cacheFile;

  private fioZIP_FILE(MemoryMappedFile file, fioZIP_CACHE_FILE cacheFile)
  {
    _file = file;
    _cacheFile = cacheFile;
  }

  public static fioZIP_FILE Open(string filePath)
  {
    if (Path.GetExtension(filePath) != ".pak")
      throw new Exception($"File is not a PAK: {filePath}");

    var cacheFile = ReadCacheFile(filePath);
    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    var file = MemoryMappedFile.CreateFromFile(fileStream, null, 0, MemoryMappedFileAccess.Read, HandleInheritability.None, false);

    return new fioZIP_FILE(file, cacheFile);
  }

  public Stream GetFileStream(string fileName)
  {
    if (!_cacheFile.TryGetValue(fileName, out var entry))
      throw new Exception($"File not found: {fileName}");

    return GetFileStream(entry);
  }

  public Stream GetFileStream(fioZIP_CACHE_FILE.ENTRY entry)
  {
    if( entry.CompressMethod == fioZIP_CACHE_FILE.COMPRESS_METHOD.STORE)
      return GetFileStreamNoCompression(entry);

    return GetFileStreamDeflate(entry);
  }

  private Stream GetFileStreamNoCompression(fioZIP_CACHE_FILE.ENTRY entry)
  {
    return _file.CreateViewStream(
      entry.Offset, 
      entry.CompressedSize,
      MemoryMappedFileAccess.Read);
  }

  private Stream GetFileStreamDeflate(fioZIP_CACHE_FILE.ENTRY entry)
  {
    using var compressedStream = _file.CreateViewStream(
      entry.Offset, 
      entry.CompressedSize, 
      MemoryMappedFileAccess.Read);

    using var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
    var fileStream = new MemoryStream();
    deflateStream.CopyTo(fileStream);

    fileStream.Position = 0;
    return fileStream;
  }

  private static fioZIP_CACHE_FILE ReadCacheFile(string filePath)
  {
    var cacheFilePath = filePath + ".cache";
    if (!File.Exists(cacheFilePath))
      throw new Exception($"No cache file found for {filePath}");

    using (var cacheFileStream = File.OpenRead(cacheFilePath))
    {
      var reader = new NativeReader(cacheFileStream, Endianness.LittleEndian);
      return Serializer<fioZIP_CACHE_FILE>.Deserialize(reader);
    }

  }

  public void Dispose()
  {
    _file?.Dispose();
    _isDisposed = true;
  }

}
