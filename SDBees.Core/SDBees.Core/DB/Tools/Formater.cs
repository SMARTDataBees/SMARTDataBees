using System.Collections.Generic;
using System.Linq;

namespace SDBees.Core.DB.Tools
{
    public class Formater
    {
        /// <summary>
        /// Concat not empty strings with the give separator
        /// </summary>
        /// <param name="separator">Separator</param>
        /// <param name="values">Values</param>
        /// <returns>Concanated values</returns>
        public static string Concat(string separator, List<string> values)
        {
            return string.Join(separator, values.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}
