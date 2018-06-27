namespace Carbon.Common
{
    /// <summary>
    /// The SecurityInformation type identifies the object-related security information being set or queried. This security information includes:
    /// <list type="">
    /// <item>The owner of an object</item>
    /// <item>The primary group of an object</item>
    /// <item>The discretionary access control list (DACL) of an object</item>
    /// <item>The system access control list (SACL) of an object</item>
    /// </list>
    /// </summary>
    public enum SecurityInformation : uint
    {
        Owner                = 0x00000001,
        Group                = 0x00000002,
        DACL                 = 0x00000004,
        SACL                 = 0x00000008,
        ProtectedDACL       = 0x80000000,
        ProtectedSACL       = 0x40000000,
        UnprotectedDACL     = 0x20000000,
        UnprotectedSACL     = 0x10000000
    }
}