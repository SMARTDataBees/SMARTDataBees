using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace SDBees.Core.GuiTools.TypeConverters
{
    public class CountryInfoTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, 
            //but allow free-form entry
            return true;
        }

        public override StandardValuesCollection
                GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetRegionDisplayNames());
        }

        private ICollection GetRegionDisplayNames()
        {
            var items = new List<string>();

            foreach (var ci in GetCultures())
            {
                if (!string.IsNullOrEmpty(ci.DisplayName))
                    items.Add(ci.DisplayName);
                /*
                Console.Write("{0,-7}", ci.Name);
                Console.Write(" {0,-3}", ci.TwoLetterISOLanguageName);
                Console.Write(" {0,-3}", ci.ThreeLetterISOLanguageName);
                Console.Write(" {0,-3}", ci.ThreeLetterWindowsLanguageName);
                Console.Write(" {0,-40}", ci.DisplayName);
                Console.WriteLine(" {0,-40}", ci.EnglishName);
                */
            }
            items.Sort();
            return items;
        }

        private static List<CultureInfo> GetCultures()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToList();
        }

        public static string GetIsoCodeForDisplayname(string displayname)
        {
            var result = "";

            var inf = GetCultures().First(it => it.DisplayName == displayname);
            if (inf != null)
                result = inf.Name;
            else
                result = "en-US";

            return result;
        }

        public static string GetDefaultCountryDisplayName(string code)
        {
            var result = "";

            var inf = GetCultures().FirstOrDefault(it => it.Name == code);
            if (inf != null)
                result = inf.DisplayName;
            else
                result = "en-US";

            return result;
        }
    }
}
