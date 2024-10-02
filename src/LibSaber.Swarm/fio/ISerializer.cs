using LibSaber.IO;

namespace LibSaber.Swarm.fio;

public interface ISerializer<TType, TFlags>
  where TFlags : Enum
{

  public TFlags Flags { get; }

  public TType Read(NativeReader reader);

}
