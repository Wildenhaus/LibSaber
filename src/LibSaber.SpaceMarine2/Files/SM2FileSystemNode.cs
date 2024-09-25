using LibSaber.FileSystem;
using LibSaber.SpaceMarine2.Structures.Resources;

namespace LibSaber.SpaceMarine2.Files;

public class SM2FileSystemNode : FileSystemNode
{

    #region Properties

    internal fioZIP_CACHE_FILE.ENTRY Entry { get; set; }
    public long SizeInBytes { get; set; }

    #endregion

    #region Constructor

    public SM2FileSystemNode( IFileSystemDevice device, fioZIP_CACHE_FILE.ENTRY entry, IFileSystemNode parent = null)
        : base( device, entry.FileName, parent )
    {
        Entry = entry;
        SizeInBytes = entry.Size;
    }

    #endregion

}
