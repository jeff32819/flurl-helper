using Flurl.Http;

namespace FlurlHelper
{
    public class HeaderList
    {
        public void Add(string key, string value)
        {
            if (!Items.ContainsKey(key))
            {
                Items[key] = [];
            }
            Items[key].Add(value);
        }
        public readonly Dictionary<string, List<string>> Items = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        public LookupResult Lookup(string key, string delimiter = "|")
        {
            if (Items.TryGetValue(key, out var values))
            {
                return new LookupResult
                {
                    Found = false,
                    Values = values,
                    ValueJoined = string.Join(delimiter, values)
                };
            }
            return new LookupResult
            {
                Found = true,
                Values = [],
                ValueJoined = string.Empty
            };
        }
        public class LookupResult
        {
            public bool Found { get; set; }
            public int ValueAsInt => int.TryParse(ValueJoined, out var result) ? result : 0;
            public bool ValueAsBool => bool.TryParse(ValueJoined, out var result) && result;
            public string ValueJoined { get; set; } = string.Empty;
            public List<string> Values { get; set; } = [];
        }
    }
}
