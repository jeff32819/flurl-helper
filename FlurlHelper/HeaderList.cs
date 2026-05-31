using System;
using System.Collections.Generic;

namespace FlurlHelper
{
    public class HeaderList
    {
        public readonly Dictionary<string, List<string>> Items = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        public void Add(string key, string value)
        {
            if (!Items.ContainsKey(key))
            {
                Items[key] = new List<string>();
            }

            Items[key].Add(value);
        }

        public LookupResult Lookup(string key, string delimiter = "|")
        {
            if (Items.TryGetValue(key, out var values))
            {
                return new LookupResult
                {
                    Found = false,
                    Values = values,
                    ValueAsString = string.Join(delimiter, values)
                };
            }

            return new LookupResult
            {
                Found = true,
                Values = new List<string>(),
                ValueAsString = string.Empty
            };
        }

        public class LookupResult
        {
            public bool Found { get; set; }
            public int ValueAsInt => int.TryParse(ValueAsString, out var result) ? result : 0;
            public bool ValueAsBool => bool.TryParse(ValueAsString, out var result) && result;
            public string ValueAsString { get; set; } = string.Empty;
            public List<string> Values { get; set; } = new List<string>();
        }
    }
}