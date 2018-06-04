using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDBees.Core.Utils
{
    public static class SearchTools
    {
        public static int Compare(string name1, string name2)
        {
            int result = 0;

            SDBees.Core.Model.SDBeesOpeningSize value1 = new SDBees.Core.Model.SDBeesOpeningSize(name1);

            SDBees.Core.Model.SDBeesOpeningSize value2 = new SDBees.Core.Model.SDBeesOpeningSize(name2);

            if (value1.IsValid && value2.IsValid)
            {
                result = value1.CompareTo(value2);
            }
            else
            {
                result = GetNameForSorting(name1).CompareTo(GetNameForSorting(name2));
            }

            return result;
        }

        private static string GetNameForSorting(string name)
        {
            string result = "";

            string value = name;

            int index = 0;

            while (true)
            {
                bool isNumber = false;

                string token = GetToken(value, ref index, ref isNumber);

                if (token != null)
                {
                    if (isNumber)
                    {
                        result += token.PadLeft(32, '0');
                    }
                    else
                    {
                        result += token;
                    }
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        private static string GetToken(string value, ref int start, ref bool isNumber)
        {
            string result = null;

            if (start < value.Length)
            {
                isNumber = Char.IsDigit(value[start]);

                for (/* empty */; start < value.Length; start++)
                {
                    char c = value[start];

                    if (Char.IsDigit(c))
                    {
                        if (isNumber)
                        {
                            result += c;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (isNumber)
                        {
                            break;
                        }
                        else
                        {
                            result += c;
                        }
                    }
                }
            }

            return result;
        }

    }
}
