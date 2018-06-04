using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override System.ComponentModel.TypeConverter.StandardValuesCollection
                GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetRegionDisplayNames());
        }

        private ICollection GetRegionDisplayNames()
        {
            List<string> items = new List<string>();

            foreach (CultureInfo ci in GetCultures())
            {
                if (!String.IsNullOrEmpty(ci.DisplayName))
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
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToList<CultureInfo>();
        }

        public static string GetIsoCodeForDisplayname(string displayname)
        {
            string result = "";

            CultureInfo inf = GetCultures().First(it => it.DisplayName == displayname);
            if (inf != null)
                result = inf.Name;
            else
                result = "en-US";

            return result;
        }

        public static string GetDefaultCountryDisplayName(string code)
        {
            string result = "";

            CultureInfo inf = GetCultures().FirstOrDefault(it => it.Name == code);
            if (inf != null)
                result = inf.DisplayName;
            else
                result = "en-US";

            return result;
        }
    }
}
