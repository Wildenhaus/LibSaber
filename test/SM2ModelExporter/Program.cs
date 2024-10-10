using System.Diagnostics;
using Assimp;
using Assimp.Unmanaged;
using LibSaber.FileSystem;
using LibSaber.SpaceMarine2.Files;

namespace SM2ModelExporter;

internal class Program
{

  public static IFileSystem FileSystem;

  static async Task Main(string[] args)
  {
    //AssimpLibrary.Instance.LoadLibrary(@"E:\Code\assimp\bin\Debug\assimp-vc143-mtd.dll");

    using (var ctx = new AssimpContext())
    {
      var test = ctx.ImportFile(@"C:\Users\rwild\Desktop\test.fbx");
    }

    InitFileSystem();
    //ExportModelAsync("cc_calgar.tpl", @"E:\test\").Wait();

    //const string OUT_PATH = @"E:\test\";
    const string OUT_PATH = null;

    //Benchmark("Sync", () => ExportModelSingleThreaded("cc_calgar.tpl", OUT_PATH));
    Benchmark("Async", () => ExportModelAsync("cc_calgar.tpl", OUT_PATH));
  }

  static void InitFileSystem()
  {
    if (FileSystem is not null)
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
    FileSystem = fs;
  }

  static async Task ExportModelSingleThreaded(string tplName, string outPath)
    => SingleThreadedModelExporter.ExportModel(FileSystem, tplName, outPath);

  static Task ExportModelAsync(string tplName, string outPath)
    => AsyncModelExporter.ExportModel(FileSystem, tplName, outPath);

  static void Benchmark(string name, Func<Task> taskFactory, int iterations = 1)
  {
    var tmpOut = Console.Out;
    Console.SetOut(TextWriter.Null);

    var sw = Stopwatch.StartNew();
    for(var i = 0; i < iterations; i++)
      taskFactory().Wait();
    sw.Stop();

    Console.SetOut(tmpOut);

    Console.WriteLine("{0} completed in {1}", name, sw.Elapsed);
    Console.WriteLine("\tAvg Iteration: {0}", sw.Elapsed / iterations);
  }

}
