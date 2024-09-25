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
    TestAllTextureFormats();
    //TestDeserializeAllTpls();

    //const string lgName = "hub_battle_barge.lg";
    //const string lgName = "pve_firestorm.lg";
    //var node = _fileSystem.EnumerateFiles().FirstOrDefault(x => Path.GetFileName(x.Name) == lgName);
    //var reader = new NativeReader(node.Open(), Endianness.LittleEndian);
    //var lg = Serializer<scnSCENE>.Deserialize(reader);

    //void CrcHash(string str)
    //  => Console.WriteLine("{0:X8} = {1}", Crc32.CalculateCrc32(str), str);

    //CrcHash("sdc_version");
    //CrcHash("binarydictionarysize");
    //CrcHash("macro");
    //CrcHash("filenamespreload");
    //CrcHash("levelids");
    //CrcHash("psos");
    //CrcHash("shader_code");

    //var hashes = new List<uint>()
    //{
    //  0x492a290f,
    //  0xb8ca1e98,
    //  0xae8fc643,
    //  0x529909c8,
    //  0x97eb2089,
    //  0x6044248d,
    //  0xeb2b14e1
    //};
    //var strings = File.ReadAllLines(@"C:\Users\rwild\Desktop\strings.txt");
    //foreach(var line in strings)
    //{
    //  var crc = Crc32.CalculateCrc32(line);
    //  if (hashes.Contains(crc))
    //    Console.WriteLine("{0:X8} = {1}",crc, line);
    //}

    //cdLIST cdl;
    //ClassList classlist;

    //{
    //  var node = _fileSystem.EnumerateFiles().FirstOrDefault(x => Path.GetFileName(x.Name) == "pve_firestorm.cd_list");
    //  var reader = new NativeReader(node.Open(), Endianness.LittleEndian);
    //  cdl = Serializer<cdLIST>.Deserialize(reader);
    //}
    //{
    //  var node = _fileSystem.EnumerateFiles().FirstOrDefault(x => Path.GetFileName(x.Name) == "pve_firestorm.class_list");
    //  var reader = new NativeReader(node.Open(), Endianness.LittleEndian);
    //  classlist = Serializer<ClassList>.Deserialize(reader);
    //}

    //foreach (var entry in cdl)
    //{
    //  if (entry.__type is null)
    //    continue;

    //  if (classlist.TplLookup.TryGetValue(entry.__type, out var tplName))
    //    continue;

    //  Console.WriteLine(entry.Name);
      //Console.WriteLine("\t{0}", tplName);
    //}

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