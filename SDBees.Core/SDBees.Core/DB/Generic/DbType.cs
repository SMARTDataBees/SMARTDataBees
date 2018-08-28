namespace SDBees.DB
{
    /// <summary>
    /// Types that can be used in columns of a table of an SQL database
    /// </summary>
    public enum DbType
    {
        /// <summary>
        /// Illegal or not initialized Type
        /// </summary>
        Unknown        = 0,
        /// <summary>
        /// Binary type
        /// </summary>
        Binary         = 1,
        /// <summary>
        /// Boolean type, for some SQL this is called "bit"
        /// </summary>
        Boolean        = 2,
        /// <summary>
        /// Byte type, 8 bit unsigned integer
        /// </summary>
        Byte           = 3,
        /// <summary>
        /// Currency type
        /// </summary>
        Currency       = 4,
        /// <summary>
        /// Date type
        /// </summary>
        Date           = 5,
        /// <summary>
        /// Date and time
        /// </summary>
        DateTime       = 6,
        /// <summary>
        /// Decimal type
        /// </summary>
        Decimal        = 7,
        /// <summary>
        /// Double precision floating point value (64 bits)
        /// </summary>
        Double         = 8,
        /// <summary>
        /// Guid type, might not be compatible for all SQL vendors. Use eGuidString instead.
        /// </summary>
        Guid           = 9,
        /// <summary>
        /// Signed 16 bit integer value (-32767 .. +32767)
        /// </summary>
        Int16          = 10,
        /// <summary>
        /// Signed 32 bit integer value (-2 billion ... + 2 billions)
        /// </summary>
        Int32          = 11,
        /// <summary>
        /// Signed 64 bit integer value (huge range)
        /// </summary>
        Int64          = 12,
        /// <summary>
        /// Single precision floating point value (32 bits)
        /// </summary>
        Single         = 13,
        /// <summary>
        /// String value, maximum size can be set
        /// </summary>
        String         = 14,
        /// <summary>
        /// String value, fixed size can be set
        /// </summary>
        StringFixed    = 15,
        /// <summary>
        /// Guid that has been introduced for SDBees that is vendor independent
        /// </summary>
        GuidString     = 16,
        /// <summary>
        /// Text that is stored as a pointer to the storing location.
        /// </summary>
        Text           = 17,
        /// <summary>
        /// Text that is stored as a pointer to the storing location.
        /// </summary>
        LongText       = 18,
        /// <summary>
        /// Custom class SDBeesOpeningSize. 
        /// </summary>
        CrossSize      = 19
    }
}