using Flurl.Http;
using FlurlHelper;


//var api = await new FluentApiBuilder("https://ssl-verify.jeff32819.dev")
//    .SetPath("api/verify")
//    .WithHeader("X-Api-Key", "81183BA8A6744B43A127472FE6813DDE")
//    .WithData(new { domainName = "jeffmathews.com", forceUpdateIfOlderThanDays = 0 })
//    .ExecutePostJsonAsync();



var api = await new FluentApiBuilder("https://api.jeff32819.dev")
    .AppendPath("api/cloudns/zones")
    .WithHeader("X-API-Key", "FirstDotNetCoreApiWithKey")
    .ExecuteGetAsync();

Console.WriteLine(api);



//var api = new FlurlClient("https://whoisjson.com/api/v1/ssl-cert-check?domain=jeffmathews.com")
//    .WithTimeout(TimeSpan.FromSeconds(300))
//    .WithHeader("Authorization", "Token=d49351b15d312f7c733fd1414daae505cc3e1f3e77ce7b91ce322f21d5a8fa38");
//var response = await api.Request().GetAsync();
//var headers = response.Headers;


//foreach (var item in headers.ToSearchableDictionary().Items)
//{
//    Console.Write(item.Key);
//    foreach (var tmp in item.Value)
//    {
//        Console.WriteLine($"\t{tmp}");
//    }
//}

Console.WriteLine();
Console.WriteLine("*** DONE *************************************************************************");
Console.WriteLine();
//var test = headers.ToSearchableDictionary();
//var value = test.Lookup("Remaining-Requests");
//Console.WriteLine(value.ValueAsString);
//Console.WriteLine(value.ValueAsBool);
//Console.WriteLine(value.ValueAsInt);
Console.ReadLine();