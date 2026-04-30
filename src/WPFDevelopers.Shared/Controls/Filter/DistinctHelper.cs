using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WPFDevelopers.Controls
{
    public static class DistinctHelper
    {
        public static List<object> GetDistinctValues(IEnumerable source, string property, Type itemType)
        {
            var prop = itemType.GetProperty(property);

            var set = new HashSet<object>();

            foreach (var item in source)
            {
                var value = prop.GetValue(item, null);
                set.Add(value);
            }

            return set.ToList();
        }
    }
}
