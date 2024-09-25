using System.IO.Compression;
using LibSaber.FileSystem;
using LibSaber.SpaceMarine2.Structures.Resources;
using static LibSaber.SpaceMarine2.Structures.Resources.fioZIP_CACHE_FILE;

namespace LibSaber.SpaceMarine2.Files;

public class SM2PckFileDevice : FileSystemDevice
{

  #region Data Members

  private readonly string _filePath;
  private readonly fioZIP_FILE _zipFile;

  #endregion

  #region Constructor

  public SM2PckFileDevice(string filePath)
  {
    _filePath = filePath;
    _zipFile = fioZIP_FILE.Open(filePath);
  }

  #endregion

  #region Overrides

  public override Stream GetStream(IFileSystemNode node)
  {
    var smNode = node as SM2FileSystemNode;
    ASSERT(smNode != null, "Node is not an SM2FileSystemNode.");

    return _zipFile.GetFileStream(smNode.Entry);
  }

  protected override IFileSystemNode OnInitializing()
      => InitNodes();

  protected override void OnDisposing()
  {
    _zipFile?.Dispose();
    base.OnDisposing();
  }

  #endregion

  #region Private Methods

  private IFileSystemNode InitNodes()
  {
    var fileName = Path.GetFileNameWithoutExtension(_filePath);
    var rootNode = new FileSystemNode(this, fileName);

    foreach (var entry in _zipFile.Entries.Values)
    {
      CreateNode(entry, rootNode);
    }

    return rootNode;
  }

  private void CreateNode(fioZIP_CACHE_FILE.ENTRY entry, IFileSystemNode parent)
  {
    var node = new SM2FileSystemNode(this, entry, parent);
    parent.AddChild(node);
  }

  #endregion

}
