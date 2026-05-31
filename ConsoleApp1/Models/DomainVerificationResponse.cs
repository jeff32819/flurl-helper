using Newtonsoft.Json;

namespace ConsoleApp1.Models;

public class DomainVerificationResponse
{
    [JsonProperty("domainName")] public string DomainName { get; set; }

    [JsonProperty("expirationDate")] public DateTimeOffset ExpirationDate { get; set; }

    [JsonProperty("expirationDateString")] public string ExpirationDateString { get; set; }

    [JsonProperty("expiresInDays")] public int ExpiresInDays { get; set; }

    [JsonProperty("errorMessage")] public string ErrorMessage { get; set; }

    [JsonProperty("isValid")] public bool IsValid { get; set; }
}