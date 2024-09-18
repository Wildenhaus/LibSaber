using System.Linq.Expressions;
using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.SpaceMarine2.Serialization;

public static class Serializer
{

  #region Public Methods

  public static T Deserialize<T>(NativeReader reader, ISerializationContext context = null)
    => Serializer<T>.Deserialize(reader, context);

  public static SM2SerializerBase<T> GetSerializer<T>()
    => Serializer<T>.GetSerializer();

  #endregion

}

public static class Serializer<T>
{

  #region Data Members

  private static Type _serializerType;
  private static Func<SM2SerializerBase<T>> _createSerializerFunc;

  #endregion

  #region Constructor

  static Serializer()
  {
    _serializerType = GetSerializerType();
    _createSerializerFunc = BuildCreateSerializerDelegate(_serializerType);
  }

  #endregion

  #region Public Methods

  public static T Deserialize(NativeReader reader, ISerializationContext context = null)
  {
    if (context is null)
      context = new SerializationContext();

    return _createSerializerFunc().Deserialize(reader, context);
  }

  public static SM2SerializerBase<T> GetSerializer()
    => _createSerializerFunc();

  #endregion

  #region Private Methods

  private static Type GetSerializerType()
  {
    var assembly = typeof(Serializer<>).Assembly;
    var types = assembly.GetTypes()
      .Where(x => x.IsAssignableTo(typeof(SM2SerializerBase<T>)))
      .ToArray();

    if (types.Length > 1)
      FAIL("More than one serializer type found for `{0}`.", typeof(T).FullName);
    if (types.Length == 0)
      FAIL("No serializer types found for `{0}`.", typeof(T).FullName);

    return types[0];
  }

  private static Func<SM2SerializerBase<T>> BuildCreateSerializerDelegate(Type serializerType)
  {
    var serializerCtor = serializerType.GetConstructor(Type.EmptyTypes);
    ASSERT(serializerCtor != null,
      "Failed to find constructor for serializer type `{0}`.",
      serializerType.FullName);

    var ctorExpression = Expression.New(serializerCtor);
    return Expression.Lambda<Func<SM2SerializerBase<T>>>(ctorExpression).Compile();
  }

  #endregion

}