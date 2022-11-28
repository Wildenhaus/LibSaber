using LibSaber.HaloCEA.Structures;
using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.HaloCEA.Tests
{

  public class SceneSerializationTests : TestBase
  {

    [Theory]
    [MemberData( nameof( GetFilePathsWithExtension ), parameters: ".Scene" )]
    public void TestSceneSerialization( string filePath )
    {
      //== Arrange ==============================
      var stream = File.OpenRead( filePath );
      var reader = new NativeReader( stream, Endianness.LittleEndian );

      //== Act ==================================
      var template = SaberScene.Deserialize( reader, new SerializationContext() );

      //== Assert ===============================

    }

  }

}
