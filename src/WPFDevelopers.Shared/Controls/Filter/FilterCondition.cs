using System.Collections.Generic;

namespace WPFDevelopers.Controls
{
    public class FilterCondition
    {
        public string PropertyName { get; set; }

        public HashSet<object> Values { get; set; }
    }
}
