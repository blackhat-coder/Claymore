using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claymore.Src.Utils
{
    public static class StringUtil
    {
        public static string ReplaceUsingIndex(this string str, int startIndex, int length, string newStr)
        {
            if(startIndex < 0 || startIndex + length > str.Length)
            {
                throw new ArgumentNullException("Range is out of bounds");
            }

            StringBuilder sb = new StringBuilder(str);
            sb.Remove(startIndex, length);
            sb.Insert(startIndex, newStr);

            return sb.ToString();
        }
    }
}
