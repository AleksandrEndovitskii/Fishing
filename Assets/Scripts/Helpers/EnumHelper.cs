using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public static class EnumHelper
    {
        public static List<T> GetValues<T>() where T : Enum
        {
            var enumValues = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            return enumValues;
        }
    }
}
