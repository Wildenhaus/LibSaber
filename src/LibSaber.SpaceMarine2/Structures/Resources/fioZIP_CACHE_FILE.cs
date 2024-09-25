using System.Collections;
using System.Diagnostics.CodeAnalysis;
using static LibSaber.SpaceMarine2.Structures.Resources.fioZIP_CACHE_FILE;

namespace LibSaber.SpaceMarine2.Structures.Resources
{

  public class fioZIP_CACHE_FILE : IReadOnlyDictionary<string, ENTRY>
  {

    private Dictionary<string, ENTRY> _entries;

    public IEnumerable<string> Keys => _entries.Keys;
    public IEnumerable<ENTRY> Values => _entries.Values;
    public int Count => _entries.Count;

    public ENTRY this[string name]
    {
      get => _entries[name];
    }

    public fioZIP_CACHE_FILE()
    {
      _entries = new Dictionary<string, ENTRY>();
    }

    internal void AddEntry(ENTRY entry)
    {
      _entries.Add(entry.FileName, entry);
    }

    public bool ContainsKey(string key)
      => _entries.ContainsKey(key);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out ENTRY value)
      => _entries.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<string, ENTRY>> GetEnumerator()
      => _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => _entries.GetEnumerator();

    public class ENTRY
    {
      public string FileName { get; set; }
      public long Offset { get; set; }
      public long Size { get; set; }
      public long CompressedSize { get; set; }
      public COMPRESS_METHOD CompressMethod { get; set; }
    }

    public enum COMPRESS_METHOD
    {
      STORE = 0,
      DEFLATE = 8,
      DEFLATE64 = 9
    }

  }

}
