using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using DirectXTexNet;
using LibSaber.FileSystem;
using LibSaber.HaloCEA.Structures;
using LibSaber.IO;
using LibSaber.SpaceMarine2.Enumerations;
using LibSaber.SpaceMarine2.Files;
using LibSaber.SpaceMarine2.Serialization;
using LibSaber.SpaceMarine2.Serialization.Geometry;
using LibSaber.SpaceMarine2.Serialization.Scripting;
using LibSaber.SpaceMarine2.Structures;
using LibSaber.SpaceMarine2.Structures.Resources;
using LibSaber.SpaceMarine2.Structures.Textures;
using Testbed;


internal class Program
{

  static FileSystem _fileSystem;

  private static void Main(string[] args)
  {
    InitFileSystem();
    //TestAllTextureFormats();
    TestDeserializeAllTpls();
    //TestDeserializeAllResources();
  }

  static void InitFileSystem()
  {
    if (_fileSystem is not null)
      return;

    Console.Write("Initializing FileSystem...");

    var fs = new FileSystem();
    var paks = Directory.EnumerateFiles(@"O:\Games\Warhammer 40000 Space Marine 2\client_pc", "*.pak", SearchOption.AllDirectories);
    foreach (var pak in paks)
    {
      var device = new SM2PckFileDevice(pak);
      device.Initialize();
      fs.AttachDevice(device);
    }
    Console.WriteLine("DONE.");
    _fileSystem = fs;
  }

  #region Tests

  static void RunTests()
  {
    InitFileSystem();
    TestDeserializeAllTpls();
    TestDeserializeAllLgs();
  }

  #region Textures

  static void TestAllTextureFormats()
  {
    foreach (var file in Directory.GetFiles(@"E:\test\"))
      File.Delete(file);

    var tester = new TextureTester(_fileSystem, @"E:\test\");
    //tester.TestAllTexturesOfType(SM2TextureFormat.R8U);
    //tester.TestAllTexturesOfType(SM2TextureFormat.R8U);
    //tester.TestAllTextures();
    //tester.TestAllTextureFormats();
    tester.BruteForceTextureFormat("story_assassination_terrain_game_material.pct");
    //tester.BruteForceTextureFormat("dry_tree_firestorm_branch_1_pivots2.pct");
  }

  #endregion

  #region TPLs

  static void TestDeserializeAllTpls()
  {
    var tpls = _fileSystem.EnumerateFiles().Where(x => Path.GetExtension(x.Name) == ".tpl");
    foreach (var tplNode in tpls)
    {
      var fname = Path.GetFileName(tplNode.Name);
      try
      {
        
        Console.Write("Deserializing {0}...", fname);
        TestDeserializeTpl(tplNode);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("SUCCESS.");
        Console.ForegroundColor = ConsoleColor.White;
      }
      catch (Exception ex)
      {
        //Console.Write("Deserializing {0}...", fname);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("FAILED.");
        Console.ForegroundColor = ConsoleColor.White;
        //throw;
      }
    }
  }

  static void TestDeserializeTpl(IFileSystemNode tplNode)
  {
    using var stream = tplNode.Open();
    var reader = new NativeReader(stream, Endianness.LittleEndian);
    var tpl = Serializer<animTPL>.Deserialize(reader);

    TestDeserializeTplData(tplNode, tpl);
  }

  static void TestDeserializeTplData(IFileSystemNode tplNode, animTPL tpl)
  {
    var tplDataName = Path.ChangeExtension(tplNode.Name, ".tpl_data");
    var tplDataNode = _fileSystem.EnumerateFiles().SingleOrDefault(x => x.Name == tplDataName);

    Stream dataStream = null;
    if (tplDataNode is null)
      dataStream = tplNode.Open();
    else
      dataStream = tplDataNode.Open();

    byte[] data = new byte[dataStream.Length];
    if(dataStream is MemoryStream ms)
    {
      data = ms.ToArray();
    }
    else
    {
      using var tmp = new MemoryStream();
      dataStream.CopyTo(tmp);
      data = tmp.ToArray();
    }
    TestDeserializeTplBuffers(tpl, data);
  }

  static unsafe void TestDeserializeTplBuffers(animTPL tpl, byte[] data)
  {
    foreach (var buffer in tpl.GeometryGraph.Buffers)
    {
      SpanReader reader;
      var startOffset = buffer.StartOffset;
      var length = (int)buffer.BufferLength;
      fixed (byte* ptr = data)
      {
        var span = new Span<byte>(ptr + startOffset, length);
        reader = new SpanReader(span);
      }

      switch (buffer.ElementType)
      {
        case GeometryElementType.Face:
          {
            //var serializer = new FaceSerializer(buffer);
            //foreach (var e in serializer.DeserializeRange(reader, 0, buffer.Count)) 
            //{
            //}
            break;
          }
        case GeometryElementType.Vertex:
        {
            var serializer = new VertexSerializer(buffer);
            var enumerator = serializer.GetEnumerator(reader, 0, buffer.Count);

            while (enumerator.MoveNext()) { }
          //foreach (var e in serializer.DeserializeRange(reader, 0, buffer.Count)) { }
          break;
        }
        case GeometryElementType.Interleaved:
          {
            var serializer = new InterleavedDataSerializer(buffer);
            var enumerator = serializer.GetEnumerator(reader, 0, buffer.Count);
            while (enumerator.MoveNext()) { }
            //foreach (var e in serializer.DeserializeRange(reader, 0, buffer.Count)) { }
            break;
          }
        default:
        {
          continue;
          throw new NotImplementedException("Unknown buffer type.");
        }
      }
    }
  }

  #endregion

  #region LevelGeo

  static void TestDeserializeAllLgs()
  {
    var lgs = _fileSystem.EnumerateFiles().Where(x => Path.GetExtension(x.Name) == ".lg");
    foreach (var lgNode in lgs)
    {
      try
      {
        var fname = Path.GetFileName(lgNode.Name);
        Console.Write("Deserializing {0}...", fname);
        TestDeserializeLg(lgNode);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("SUCCESS.");
        Console.ForegroundColor = ConsoleColor.White;
      }
      catch (Exception ex)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("FAILED.");
        Console.ForegroundColor = ConsoleColor.White;
      }
    }
  }

  static void TestDeserializeLg(IFileSystemNode lgNode)
  {
    using var stream = lgNode.Open();
    var reader = new NativeReader(stream, Endianness.LittleEndian);
    var lg = Serializer<scnSCENE>.Deserialize(reader);
  }

  #endregion

  #region Resources

  static void TestDeserializeAllResources()
  {
    var res = _fileSystem.EnumerateFiles().Where(x => Path.GetExtension(x.Name) == ".resource");
    int i = 0;
    var sw = Stopwatch.StartNew();
    foreach (var resNode in res)
    {
      i++;
      //var fname = Path.GetFileName(resNode.Name);
      //try
      //{

      //  //Console.Write("Deserializing {0}...", fname);
        TestDeserializeResource(resNode);
      //  //Console.ForegroundColor = ConsoleColor.Green;
      //  //Console.WriteLine("SUCCESS.");
      //  //Console.ForegroundColor = ConsoleColor.White;
      //}
      //catch (Exception ex)
      //{
      //  Console.ForegroundColor = ConsoleColor.Red;
      //  Console.WriteLine("FAILED.");
      //  Console.ForegroundColor = ConsoleColor.White;
      //  //throw;
      //}
    }
    sw.Stop();
    Console.WriteLine("Deserialized {0} in {1}.", i, sw.Elapsed );
  }

  static void TestDeserializeResource(IFileSystemNode resNode)
  {
    using var stream = resNode.Open();
    var reader = new NativeReader(stream, Endianness.LittleEndian);
    var res = Serializer<resDESC>.Deserialize(reader);
  }

  #endregion

  #endregion

}