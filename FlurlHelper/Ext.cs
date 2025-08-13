using Flurl.Util;

namespace FlurlHelper;

public static class Ext
{
    /// <summary>
    /// Converts the response headers into a dictionary with searchable keys and list of values.
    /// </summary>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static HeaderList ToSearchableDictionary(this IReadOnlyNameValueList<string> headers)
    {
        var arr = new HeaderList();
        foreach (var (name, value) in headers)
        {
            arr.Add(name, value);
        }
        return arr;
    }
}