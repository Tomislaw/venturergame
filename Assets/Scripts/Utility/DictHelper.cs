using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utility
{
    public static class DictHelper
    {
        public static TV GetValue<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV))
        {
            TV value;
            return dict.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static TD GetValueCasting<TK, TV, TD>(this IDictionary<TK, TV> dict, TK key, TD defaultValue = default(TD))
        {
            TV value;
            if (dict.TryGetValue(key, out value))
            {
                if (value.GetType().Equals(typeof(TD)))
                {
                    return (TD)(object)value;
                }
                else
                {
                    return defaultValue;
                }
            }
            else
                return defaultValue;
        }
    }
}