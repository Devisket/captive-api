using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Utility
{
    public static class EnumerableHelper
    {
        public static bool ContainsAny(this IEnumerable<Guid> source, IEnumerable<Guid>? target)
        {
            foreach (var item in target) 
            {
                if(source.Contains(item))
                    return true;
            }

            return false;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T>? target)
        {
            foreach (var item in target)
            {
                if (source.Contains(item))
                    return true;
            }

            return false;
        }

    }
}
