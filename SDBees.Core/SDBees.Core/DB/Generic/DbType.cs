using System.Xml.Serialization;

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
        [XmlEnum("0")]
        Unknown        = 0,
        /// <summary>
        /// Binary type
        /// </summary>
        [XmlEnum("1")]
        Binary = 1,
        /// <summary>
        /// Boolean type, for some SQL this is called "bit"
        /// </summary>
        [XmlEnum("2")]
        Boolean        = 2,
        /// <summary>
        /// Byte type, 8 bit unsigned integer
        /// </summary>
        [XmlEnum("3")]
        Byte           = 3,
        /// <summary>
        /// Currency type
        /// </summary>
        [XmlEnum("4")]
        Currency       = 4,
        /// <summary>
        /// Date type
        /// </summary>
        [XmlEnum("5")]
        Date           = 5,
        /// <summary>
        /// Date and time
        /// </summary>
        [XmlEnum("6")]
        DateTime       = 6,
        /// <summary>
        /// Decimal type
        /// </summary>
        [XmlEnum("7")]
        Decimal        = 7,
        /// <summary>
        /// Double precision floating point value (64 bits)
        /// </summary>
        [XmlEnum("8")]
        Double         = 8,
        /// <summary>
        /// Guid type, might not be compatible for all SQL vendors. Use eGuidString instead.
        /// </summary>
        [XmlEnum("9")]
        Guid           = 9,
        /// <summary>
        /// Signed 16 bit integer value (-32767 .. +32767)
        /// </summary>
        [XmlEnum("10")]
        Int16          = 10,
        /// <summary>
        /// Signed 32 bit integer value (-2 billion ... + 2 billions)
        /// </summary>
        [XmlEnum("11")]
        Int32          = 11,
        /// <summary>
        /// Signed 64 bit integer value (huge range)
        /// </summary>
        [XmlEnum("12")]
        Int64          = 12,
        /// <summary>
        /// Single precision floating point value (32 bits)
        /// </summary>
        [XmlEnum("13")]
        Single         = 13,
        /// <summary>
        /// String value, maximum size can be set
        /// </summary>
        [XmlEnum("14")]
        String         = 14,
        /// <summary>
        /// String value, fixed size can be set
        /// </summary>
        [XmlEnum("15")]
        StringFixed    = 15,
        /// <summary>
        /// Guid that has been introduced for SDBees that is vendor independent
        /// </summary>
        [XmlEnum("16")]
        GuidString     = 16,
        /// <summary>
        /// Text that is stored as a pointer to the storing location.
        /// </summary>
        [XmlEnum("17")]
        Text           = 17,
        /// <summary>
        /// Text that is stored as a pointer to the storing location.
        /// </summary>
        [XmlEnum("18")]
        LongText       = 18,
        /// <summary>
        /// Custom class SDBeesOpeningSize. 
        /// </summary>
        [XmlEnum("19")]
        CrossSize      = 19
    }
}