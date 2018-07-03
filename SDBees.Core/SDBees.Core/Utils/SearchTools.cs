using SDBees.Core.Model;

namespace SDBees.Core.Utils
{
    public static class SearchTools
    {
        public static int Compare(string name1, string name2)
        {
            var result = 0;

            var value1 = new SDBeesOpeningSize(name1);

            var value2 = new SDBeesOpeningSize(name2);

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
            var result = "";

            var value = name;

            var index = 0;

            while (true)
            {
                var isNumber = false;

                var token = GetToken(value, ref index, ref isNumber);

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
                isNumber = char.IsDigit(value[start]);

                for (/* empty */; start < value.Length; start++)
                {
                    var c = value[start];

                    if (char.IsDigit(c))
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
                        result += c;
                    }
                }
            }

            return result;
        }

    }
}
