namespace LibSaber.IO.Tests;

public class NativeReaderSeekTests : NativeReaderTestsBase
{

  [Fact]
  public void NativeReader_seeks_forward_and_backwards()
  {
    const int BUFFER_SIZE = 16;
    const int ELEM_COUNT = 1_000_000;
    const int ITERATION_COUNT = 1_000_000;

    var random = new Random();
    var stream = InitMemoryStream(ELEM_COUNT);
    var reader = new NativeReader(stream, bufferSize: BUFFER_SIZE);

    long GetNumPosition(int num) => num * sizeof(int);
    long GetPositionOffset(long currentPosition, int num) => GetNumPosition(num) - currentPosition;

    int expectedNum = 0;

    for (var i = 0; i < ITERATION_COUNT; i++)
    {
      var seekSize = random.Next(-ELEM_COUNT, ELEM_COUNT);
      expectedNum = (expectedNum + seekSize) % ELEM_COUNT;

      // Ensure expectedNum is non-negative by normalizing it within bounds
      if (expectedNum < 0)
        expectedNum += ELEM_COUNT;

      var offset = GetPositionOffset(reader.Position, expectedNum);
      reader.Position += offset;
      var readValue = reader.ReadInt32();

      Assert.Equal(expectedNum, readValue);
    }
  }

  private MemoryStream InitMemoryStream(int elemCount)
  {
    var stream = new MemoryStream();

    for (var i = 0; i < elemCount; i++)
      stream.Write(BitConverter.GetBytes(i));

    stream.Position = 0;
    return stream;
  }

}
