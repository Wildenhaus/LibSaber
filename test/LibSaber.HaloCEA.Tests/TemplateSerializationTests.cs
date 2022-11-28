using LibSaber.HaloCEA.Structures;
using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.HaloCEA.Tests
{

  public class TemplateSerializationTests : TestBase
  {

    [Theory]
    [MemberData( nameof( GetFilePathsWithExtension ), parameters: ".Template" )]
    public void TestTemplateSerialization( string filePath )
    {
      //== Arrange ==============================
      var stream = File.OpenRead( filePath );
      var reader = new NativeReader( stream, Endianness.LittleEndian );

      //== Act ==================================
      var template = Template.Deserialize( reader, new SerializationContext() );

      //== Assert ===============================
      Assert.Equal( reader.Position, stream.Length );
    }

  }

}
