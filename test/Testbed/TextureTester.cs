using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DirectXTexNet;
using LibSaber.FileSystem;
using LibSaber.IO;
using LibSaber.SpaceMarine2.Enumerations;
using LibSaber.SpaceMarine2.Serialization;
using LibSaber.SpaceMarine2.Structures;
using LibSaber.SpaceMarine2.Structures.Resources;
using static LibSaber.Assertions;

namespace Testbed;


public class TextureTester
{

  private FileSystem _fileSystem;
  private string _outPath;

  public TextureTester(FileSystem fileSystem, string outPath)
  { 
    _fileSystem = fileSystem; 
    _outPath = outPath;
  }

  public void TestAllTextures()
  {
    var pctRscNodes = _fileSystem.EnumerateFiles().Where(x => x.Name.EndsWith(".pct.resource")).ToArray();

    var total = pctRscNodes.Length;
    var current = 0;

    var tasks = new List<Task>();
    foreach(var node in pctRscNodes)
    {
      tasks.Add(Task.Run(() =>
      {
        try
        {
          Interlocked.Increment(ref current);
          Console.Title = $"Testing textures: {current}/{total}";
          var ctx = ConvertFSResourceNodeToContext(node);
          TestConvertTexture(ctx);
        }
        catch
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine("{0} FAILED", node.Name);
          Console.ForegroundColor = ConsoleColor.White;
        }
      }));
    }

    Task.WaitAll(tasks.ToArray());
  }

  public void TestAllTexturesOfType(SM2TextureFormat format)
  {
    var resPctNodes = _fileSystem.EnumerateFiles()
      .Where(x => x.Name.EndsWith(".pct.resource"))
      .ToArray();

    foreach (var node in resPctNodes)
    {
      using var stream = node.Open();
      var reader = new NativeReader(stream, Endianness.LittleEndian);
      var res = Serializer<resDESC>.Deserialize(reader) as resDESC_PCT;

      if (res.TextureFormat != format)
        continue;

      var ctx = ConvertFSResourceNodeToContext(node);
      TestConvertTexture(ctx);
    }
  }

  public void TestTextures(IEnumerable<string> names)
  {
    foreach(var name in names)
    {
      TestConvertTexture(name);
    }
  }

  public void TestAllTextureFormats()
  {
    var formatDict = BuildTextureFormatSampleDict();

    foreach( var ctx in formatDict.Values)
    {
      Console.Write("Processing {0} (Format: {1})...", ctx.Name, ctx.ResourceDescription.TextureFormat);
      try
      {
        TestConvertTexture(ctx);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("SUCCESS");
        Console.ForegroundColor = ConsoleColor.White;
      }
      catch
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("FAILED");
        Console.ForegroundColor = ConsoleColor.White;
      }
    }
  }

  public void BruteForceTextureFormat(string name)
  {
    var formats = Enum.GetValues<DXGI_FORMAT>();
    var tasks = new List<Task>();
    foreach(var format in formats)
    {
      tasks.Add(Task.Run(() =>
      {
        try
        {
          var currentFmt = format;
          var formatSelector = (SM2TextureFormat fmt) => currentFmt;
          TestConvertTexture(name, formatSelector);
          Console.WriteLine("Format succeeded: {0}", format);
        }
        catch { }
      }));

      Task.WaitAll(tasks.ToArray());
    }
  }
  
  public void TestConvertTexture(string name, Func<SM2TextureFormat, DXGI_FORMAT> formatSelector = null)
  {
    name = Path.GetFileNameWithoutExtension(name);
    name = Path.GetFileNameWithoutExtension(name);
    name += ".pct.resource";

    var node = _fileSystem.EnumerateFiles().SingleOrDefault(x => Path.GetFileName(x.Name) == name);
    ASSERT(node is not null);

    var ctx = ConvertFSResourceNodeToContext(node);
    TestConvertTexture(ctx, formatSelector);
  }

  private void TestConvertTexture(TextureContext context, Func<SM2TextureFormat, DXGI_FORMAT> formatSelector = null)
  {
    var outFileName =  context.Name;
    var outFilePath = Path.Combine(_outPath, outFileName);

    LoadRemainingData(context);
    ProcessTexture(context, outFilePath, formatSelector);
  }

  #region Helpers

  Dictionary<SM2TextureFormat, TextureContext> BuildTextureFormatSampleDict()
  {
    var formatDict = new Dictionary<SM2TextureFormat, TextureContext>();
    var resPctNodes = _fileSystem.EnumerateFiles()
      .Where(x => x.Name.EndsWith(".pct.resource"))
      .OrderBy(x => Guid.NewGuid().GetHashCode());

    foreach (var node in resPctNodes)
    {
      using var stream = node.Open();
      var reader = new NativeReader(stream, Endianness.LittleEndian);
      var res = Serializer<resDESC>.Deserialize(reader) as resDESC_PCT;

      if (!formatDict.ContainsKey(res.TextureFormat))
      {
        var ctx = new TextureContext
        {
          ResourceNode = node,
          ResourceDescription = res
        };
        formatDict.Add(res.TextureFormat, ctx);
      }
    }

    return formatDict;
  }

  TextureContext ConvertFSResourceNodeToContext(IFileSystemNode node)
  {
    ASSERT(node is not null);
    using var stream = node.Open();
    var reader = new NativeReader(stream, Endianness.LittleEndian);
    var res = Serializer<resDESC>.Deserialize(reader) as resDESC_PCT;

    return new TextureContext
    {
      ResourceNode = node,
      ResourceDescription = res
    };
  }

  void LoadRemainingData(TextureContext ctx)
  {
    if(!string.IsNullOrWhiteSpace(ctx.ResourceDescription.pct))
    {
      var pctName = ctx.ResourceDescription.pct;
      var pctNode = _fileSystem.EnumerateFiles().SingleOrDefault(x => x.Name.EndsWith(pctName));
      ASSERT(pctNode != null);

      using var stream = pctNode.Open();
      var reader = new NativeReader(stream, Endianness.LittleEndian);
      ctx.Picture = Serializer<pctPICTURE>.Deserialize(reader);
    }
    else
    {
      var mipStream = ctx.MipStream = new MemoryStream();
      foreach (var mipName in ctx.ResourceDescription.mipMaps)
      {
        var mipNode = _fileSystem.EnumerateFiles().SingleOrDefault(x => Path.GetFileName(x.Name) == mipName);
        ASSERT(mipNode != null);

        using var stream = mipNode.Open();
        stream.CopyTo(mipStream);
      }

      mipStream.Position = 0;
    }
  }

  void ProcessTexture(TextureContext ctx, string outPath, Func<SM2TextureFormat, DXGI_FORMAT> formatSelector = null)
  {
    if (formatSelector is null)
      formatSelector = ConvertFormat;

    var width = ctx.ResourceDescription.header.sx;
    var height = ctx.ResourceDescription.header.sy;
    var depth = ctx.ResourceDescription.header.sz;
    var nFaces = ctx.ResourceDescription.header.nFaces;
    var nMips = ctx.ResourceDescription.header.nMipMap;
    var format = formatSelector(ctx.ResourceDescription.TextureFormat);

    outPath += $"{ctx.ResourceDescription.TextureFormat}_{format}.dds";

    byte[] srcData;
    if (ctx.Picture is not null)
      srcData = ctx.Picture.Data;
    else
      srcData = ctx.MipStream.ToArray();

    var img = TexHelper.Instance.Initialize2D(format, width, height, depth * nFaces, nMips, CP_FLAGS.NONE);

    var srcDataLen = srcData.Length;
    var pDest = img.GetPixels();
    var pDestLen = img.GetPixelsSize();
    ASSERT(pDestLen >= srcDataLen, "Source data will not fit");

    Marshal.Copy(srcData, 0, pDest, srcDataLen);

    using var ddsStream = img.SaveToDDSMemory(DDS_FLAGS.NONE);
    using var outStream = File.Create(outPath);
    ddsStream.CopyTo(outStream);
    outStream.Flush();
  }

  private DXGI_FORMAT ConvertFormat(SM2TextureFormat textureFormat)
  {
    switch(textureFormat)
    {
      case SM2TextureFormat.ARGB8888:
        return DXGI_FORMAT.B8G8R8A8_UNORM;
      case SM2TextureFormat.ARGB16161616U:
        return DXGI_FORMAT.R16G16B16A16_UNORM;
      case SM2TextureFormat.BC6U:
        return DXGI_FORMAT.BC6H_UF16;
      case SM2TextureFormat.BC7:
      case SM2TextureFormat.BC7A:
        return DXGI_FORMAT.BC7_UNORM;
      case SM2TextureFormat.DXN:
        return DXGI_FORMAT.BC5_UNORM;
      case SM2TextureFormat.DXT5A:
        return DXGI_FORMAT.BC4_UNORM;
      case SM2TextureFormat.AXT1:
      case SM2TextureFormat.OXT1:
        return DXGI_FORMAT.BC1_UNORM;
      case SM2TextureFormat.R8U:
        return DXGI_FORMAT.R8_UNORM;
      case SM2TextureFormat.R16:
        return DXGI_FORMAT.R16_SNORM;
      case SM2TextureFormat.R16G16:
        return DXGI_FORMAT.R16G16_SINT;
      case SM2TextureFormat.RGBA16161616F:
        return DXGI_FORMAT.R16G16B16A16_FLOAT;
      case SM2TextureFormat.XT5:
        return DXGI_FORMAT.BC3_UNORM;
      case SM2TextureFormat.XRGB8888:
        return DXGI_FORMAT.B8G8R8X8_UNORM;

      default:
        throw new Exception($"Unknown format: {textureFormat}");
    }
  }

  #endregion

  class TextureContext
  {
    public string Name
    {
      get
      {
        var outFileName = Path.GetFileNameWithoutExtension(ResourceNode.Name);
        outFileName = Path.GetFileNameWithoutExtension(outFileName);
        return outFileName;
      }
    }

    public IFileSystemNode ResourceNode { get; set; }
    public MemoryStream MipStream { get; set; }
    public resDESC_PCT ResourceDescription { get; set; }
    public pctPICTURE Picture { get; set; }
  }

}
