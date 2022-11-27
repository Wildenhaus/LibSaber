using LibSaber.HaloCEA.Structures;
using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.HaloCEA.Tests
{

  public class TemplateSerializationTests
  {

    [Fact]
    public void TestTemplateSerialization()
    {
      //== Arrange ==============================
      var stream = File.OpenRead( @"G:\h1a\x\a10\marine.Template" );
      var reader = new NativeReader( stream, Endianness.LittleEndian );

      //== Act ==================================
      var template = Data_02E4.Deserialize( reader, new SerializationContext() );

      //== Assert ===============================

    }

  }

}
