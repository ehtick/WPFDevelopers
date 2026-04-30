using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WPFDevelopers.Controls
{
    public static class PropertyAccessor<T>
    {
        static Dictionary<string, Func<T, object>> _cache
            = new Dictionary<string, Func<T, object>>();

        public static Func<T, object> Get(string property)
        {
            if (_cache.TryGetValue(property, out var getter))
                return getter;

            var param = Expression.Parameter(typeof(T));

            var body = Expression.Convert(
                Expression.PropertyOrField(param, property),
                typeof(object));

            getter = Expression
                .Lambda<Func<T, object>>(body, param)
                .Compile();

            _cache[property] = getter;

            return getter;
        }
    }
}
