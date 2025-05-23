﻿using LibSaber.IO;

namespace LibSaber.SpaceMarine2.Serialization.Scripting
{

  public class StaticBinaryScriptingSerializer<T> : BinaryScriptingSerializer<T>
    where T : class, new()
  {

    #region Data Members

    private readonly Dictionary<Type, IScriptingSerializer> _serializerCache
      = new Dictionary<Type, IScriptingSerializer>();

    #endregion

    #region Overrides

    protected override Dictionary<Type, IScriptingSerializer> GetSerializerCache()
      => _serializerCache;

    protected override void ReadProperty( NativeReader reader, T obj )
    {
      var propertyName = reader.ReadNullTerminatedString();
      var unk_01 = reader.ReadUInt32(); // TODO
      var dataType = ReadDataType( reader );

      var propertyValue = ReadValue( reader, dataType, propertyName );
      SetPropertyValue( obj, propertyName, propertyValue );
    }

    #endregion

  }

}