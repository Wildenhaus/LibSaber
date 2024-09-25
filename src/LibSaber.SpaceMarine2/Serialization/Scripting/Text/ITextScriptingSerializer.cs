namespace LibSaber.SpaceMarine2.Serialization.Scripting
{

  public interface ITextScriptingSerializer : IScriptingSerializer
  {

    dynamic Deserialize( StringReader reader );

  }

  public interface ITextConfigurationSerializer<T> : ITextScriptingSerializer
    where T : class, new()
  {

    new T Deserialize( StringReader reader );

  }

}
