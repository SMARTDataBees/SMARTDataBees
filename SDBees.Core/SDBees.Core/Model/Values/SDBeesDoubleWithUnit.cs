using System.Runtime.Serialization;

namespace SDBees.Core.Model
{

    public enum SDBeesUnitType
    {
        Text,
        Double,
        Integer,
        Boolean,
        DoubleWithUnit,
        Enumeration
    }

    [DataContract]
    public class SDBeesDoubleWithUnit
    {
        public SDBeesDoubleWithUnit()
        {
            Value = 0.0;
            Unit = "";
        }

        public SDBeesDoubleWithUnit(double value, string unit)
        {
            Value = value;
            Unit = unit;
        }

        /// <summary>
        /// Der Double Wert
        /// </summary>
        [DataMember]
        public double Value { get; set; }

        /// <summary>
        /// Die einheit
        /// </summary>
        [DataMember]
        public string Unit { get; set; }
    }
}
