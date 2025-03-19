using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extentions
{
    public static class ListStringExtentions
    {
        public static string JoinString(this List<string> list)
        {
            StringBuilder res = new StringBuilder();
            foreach (var item in list)
            {
                res.AppendLine(item);                                                  
            }

            return res.ToString();
        }
    }
}
