using System.Collections.Generic;
using Newtonsoft.Json;

namespace Extensions
{
    public static class ReflectionExtensions
    {
        public static bool ValueEquals<T>(this List<T> list1, List<T> list2)
        {
            // TODO: temp solution
            var list1Json = JsonConvert.SerializeObject(list1);
            var list2Json = JsonConvert.SerializeObject(list2);
            var result = list1Json.Equals(list2Json);

            return result;
        }
    }
}
