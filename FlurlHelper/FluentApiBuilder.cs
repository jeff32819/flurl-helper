using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Flurl.Http;

using Newtonsoft.Json;

using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FlurlHelper
{

    public class FluentApiBuilder(string baseUrl)
    {
        private readonly string _baseUrl = baseUrl.TrimEnd('/');
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // UPGRADE: Changed from <string, string> to <string, object> to preserve integers and booleans in JSON bodies
        private readonly Dictionary<string, object> _payload = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        // 1. Replace PathSegmentBuilder with a native List
        private readonly List<object> _pathSegments = new List<object>();

        // 2. The streamlined fluent method
        public FluentApiBuilder AppendPath(params object[] segments)
        {
            _pathSegments.AddRange(segments);
            return this;
        }


        // UPGRADE: Accept object so you can pass ints, bools, etc.
        public FluentApiBuilder AddParam(string key, object value)
        {
            _payload[key] = value;
            return this;
        }
        public FluentApiBuilder WithBearerToken(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                _headers["Authorization"] = $"Bearer {token}";
            }
            return this;
        }
        public FluentApiBuilder WithData<TRequest>(TRequest data)
        {
            if (data == null) return this;

            var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
            var jsonString = JsonSerializer.Serialize(data, options);
            var dataDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);

            if (dataDict == null) return this;

            foreach (var kvp in dataDict)
            {
                // UPGRADE: Safely unwrap JsonElement to native types instead of forcing .ToString()
                _payload[kvp.Key] = kvp.Value.ValueKind switch
                {
                    JsonValueKind.Number => kvp.Value.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => kvp.Value.ToString()
                };
            }

            return this;
        }

        public FluentApiBuilder WithHeader(string name, string value)
        {
            _headers[name] = value;
            return this;
        }

        public FluentApiBuilder WithHeaders(IDictionary<string, string> headersToAdd)
        {
            if (headersToAdd == null) return this;
            foreach (var kvp in headersToAdd) _headers[kvp.Key] = kvp.Value;
            return this;
        }

        // UPGRADE: Added CancellationTokens to execution methods
        public async Task<string> ExecuteGetAsync(CancellationToken ct = default)
        {
            // FIX: Add 'cancellationToken:' before 'ct'
            return await ExecuteCoreAsync(req => req.SetQueryParams(_payload).GetAsync(cancellationToken: ct));
        }
        public async Task<TResponse> ExecuteGetAsync<TResponse>(CancellationToken ct = default)
        {
            return Deserialize<TResponse>(await ExecuteGetAsync(ct));
        }

        public async Task<string> ExecutePostJsonAsync(CancellationToken ct = default)
        {
            return await ExecuteCoreAsync(req => req.PostJsonAsync(_payload, cancellationToken: ct));
        }

        public async Task<string> ExecutePostUrlEncodedAsync(CancellationToken ct = default)
        {
            return await ExecuteCoreAsync(req => req.PostUrlEncodedAsync(_payload, cancellationToken: ct));
        }

        public async Task<TResponse> ExecutePostJsonAsync<TResponse>(CancellationToken ct = default)
        {
            return Deserialize<TResponse>(await ExecutePostJsonAsync(ct));
        }

        public async Task<TResponse> ExecutePostUrlEncodedAsync<TResponse>(CancellationToken ct = default)
        {
            return Deserialize<TResponse>(await ExecutePostUrlEncodedAsync(ct));
        }

        // UPGRADE: Centralized FlurlHttpException handling
        private async Task<string> ExecuteCoreAsync(Func<IFlurlRequest, Task<IFlurlResponse>> postAction)
        {
            var request = _baseUrl
                .WithHeaders(_headers)
                .AppendPathSegments(_pathSegments);

            try
            {
                var response = await postAction(request);
                var rawJson = await response.GetStringAsync();
                Debug.Print($"RAW API RESPONSE [{request.Url}] --------------------------------");
                Debug.Print(rawJson);

                return rawJson;
            }
            catch (FlurlHttpException ex)
            {
                // Extract the actual error message from the API before throwing
                var errorBody = await ex.GetResponseStringAsync();
                Debug.Print($"API ERROR [{request.Url}]: {errorBody}");
                throw;
            }
        }

        // UPGRADE: Standardized on System.Text.Json to match your WithData method
        private TResponse Deserialize<TResponse>(string rawJson)
        {
            try
            {
                return JsonSerializer.Deserialize<TResponse>(rawJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                throw new Exception("JSON Parsing Failed: " + ex.Message, ex);
            }
        }
    }
}