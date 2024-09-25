namespace LibSaber.SpaceMarine2.Structures
{

  public class ClassList : List<ClassList.Entry>
  {
    public Dictionary<string, string> TplLookup { get; set; }

    public class Entry
    {
      public string Name { get; set; }
      public Dictionary<string, dynamic> PS { get; set; }

    }

  }
}
