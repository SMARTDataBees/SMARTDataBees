using System;
using System.ComponentModel;

namespace Carbon.Common
{
    [Flags]
    public enum SecurityAccessRights : uint
    {
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Any)]
        [Description("Delete")]
        Delete                    = 0x00010000,
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Any)]
        [Description("Read Permissions")]
        ReadControl              = 0x00020000,
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Any)]
        [Description("Change Permissions")]
        WriteDac                 = 0x00040000,
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Any)]
        [Description("Take Ownership")]
        WriteOwner               = 0x00080000,

        Synchronize               = 0x00100000,
        StandardRightsRequired  = 0x000F0000,
        StandardRightsRead      = ReadControl,
        StandardRightsWrite     = ReadControl,
        StandardRightsExecute   = ReadControl,
        StandardRightsAll       = 0x001F0000,
        SPECIFIC_RIGHTS_ALL       = 0x0000FFFF,
        AccessSystemSecurity    = 0x01000000,
        MaximumAllowed           = 0x02000000,
        GenericRead              = 0x80000000,
        GenericWrite             = 0x40000000,
        GenericExecute           = 0x20000000,
        GenericAll               = 0x10000000,

        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Files | WindowsUIPermission.AppliesTo.Pipes)]
        [Description("Read Data")]
        FILE_READ_DATA            = 0x0001,    // file & pipe		
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Folders)]
        [Description("List Folder")]
        FILE_LIST_DIRECTORY       = 0x0001,    // directory
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Files | WindowsUIPermission.AppliesTo.Pipes)]
        [Description("Write Data")]
        FILE_WRITE_DATA           = 0x0002,    // file & pipe
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Folders)]
        [Description("Create Files")]
        FILE_ADD_FILE             = 0x0002,    // directory
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Files)]
        [Description("Append Data")]
        FILE_APPEND_DATA          = 0x0004,    // file
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Folders)]
        [Description("Create Folders")]
        FILE_ADD_SUBDIRECTORY     = 0x0004,    // directory
		
        FILE_CREATE_PIPE_INSTANCE = 0x0004,    // named pipe
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Files | WindowsUIPermission.AppliesTo.Folders)]
        [Description("Read Extended Attributes")]
        FILE_READ_EA              = 0x0008,    // file & directory
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Files | WindowsUIPermission.AppliesTo.Folders)]
        [Description("Write Extended Attributes")]
        FILE_WRITE_EA             = 0x0010,    // file & directory
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Files)]
        [Description("Execute File")]	
        FILE_EXECUTE              = 0x0020,    // file
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Folders)]
        [Description("Traverse Folder")]
        FILE_TRAVERSE             = 0x0020,    // directory
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.Folders)]
        [Description("Delete Subfolders and File permissions")]
        FILE_DELETE_CHILD         = 0x0040,    // directory
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.All)]
        [Description("Read Attributes")]
        FILE_READ_ATTRIBUTES      = 0x0080,    // all
		
        [WindowsUIPermission(WindowsUIPermission.AppliesTo.All)]
        [Description("Write Attributes")]
        FILE_WRITE_ATTRIBUTES     = 0x0100,    // all
		
        [Description("All Access")]
        FILE_ALL_ACCESS			  = StandardRightsRequired | Synchronize | 0x1FF,
		
        [Description("Read")]
        FILE_GENERIC_READ         = StandardRightsRead | FILE_READ_DATA | FILE_READ_ATTRIBUTES | FILE_READ_EA | Synchronize,

        [Description("Write")]
        FILE_GENERIC_WRITE        = StandardRightsWrite | FILE_WRITE_DATA | FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_APPEND_DATA | Synchronize,

        [Description("Execute")]
        FILE_GENERIC_EXECUTE      = StandardRightsExecute | FILE_READ_ATTRIBUTES | FILE_EXECUTE | Synchronize
    }
}