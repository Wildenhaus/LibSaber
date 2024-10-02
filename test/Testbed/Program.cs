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
    //TestDeserializeAllTpls();
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
      try
      {
        var fname = Path.GetFileName(tplNode.Name);
        Console.Write("Deserializing {0}...", fname);
        TestDeserializeTpl(tplNode);
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

  static void TestDeserializeTpl(IFileSystemNode tplNode)
  {
    using var stream = tplNode.Open();
    var reader = new NativeReader(stream, Endianness.LittleEndian);
    var tpl = Serializer<animTPL>.Deserialize(reader);
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

  #endregion

}