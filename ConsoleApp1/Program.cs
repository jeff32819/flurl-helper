


using System.Diagnostics;
using System.Runtime.InteropServices;

using Flurl.Http;
using FlurlHelper;


var api = new FlurlClient($"https://whoisjson.com/api/v1/ssl-cert-check?domain=jeffmathews.com")
    .WithTimeout(TimeSpan.FromSeconds(300))
    .WithHeader("Authorization", "Token=d49351b15d312f7c733fd1414daae505cc3e1f3e77ce7b91ce322f21d5a8fa38");
var response = await api.Request().GetAsync();
var headers = response.Headers;




foreach(var item in headers.ToSearchableDictionary().Items)
{
    Console.Write(item.Key);
    foreach (var tmp in item.Value)
    {
        Console.WriteLine($"\t{tmp}");
    }
}

Console.WriteLine();
Console.WriteLine("00000000000000000000000000000000000000000000000");
Console.WriteLine();


var test = headers.ToSearchableDictionary();
var value = test.Lookup("Remaining-Requests");
Console.WriteLine(value.ValueJoined);
Console.WriteLine(value.ValueAsBool);
Console.WriteLine(value.ValueAsInt);
Console.WriteLine();