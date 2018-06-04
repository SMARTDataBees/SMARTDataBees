using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SDBees.Core.Model.Math
{
    public enum LengthUnits
    {
        //Architectural, // feet + inch in Bruch -> 7" 4 1/4'
        Engineering, // feet in engineering (decimal) view -> 6.58
        //Fractional, // Inch mit Bruch -> 34 1/4
        Millimeters,
        Meters,
        Inches
    }

    public enum AreaUnits
    {
        Square_Engineering,
        Square_Millimeters,
        Square_Meters,
        Square_Inches
    }

    //public class SDBeesUnits
    //{
    //    [DefaultValue(LengthUnits.Engineering)]
    //    public LengthUnits SDBeesLengthUnits { get; set; }

    //    [DefaultValue(AreaUnits.Square_Engineering)]
    //    public AreaUnits SDBeesAreaUnits { get; set; }
    //}
}
