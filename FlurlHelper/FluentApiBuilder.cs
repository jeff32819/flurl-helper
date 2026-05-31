using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FlurlHelper
{
    public class FluentApiBuilder
    {
        private readonly string _baseUrl;

        // This dictionary holds your headers until execution.
        // We use StringComparer.OrdinalIgnoreCase because HTTP headers are case-insensitive!
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _payload = new Dictionary<string, string>();


        private string _path;

        public FluentApiBuilder(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        // 1. Fluent method for Authentication (Overrides defaults)
        public FluentApiBuilder SetAuth(string authId, string password)
        {
            _payload["auth-id"] = authId;
            _payload["auth-password"] = password;
            return this; // Returning 'this' is the magic of Fluent APIs
        }

        // 2. Fluent method for the Endpoint
        public FluentApiBuilder SetPath(string path)
        {
            _path = path;
            return this;
        }

        public FluentApiBuilder AddParam(string key, string value)
        {
            _payload[key] = value;
            return this;
        }


        // 3. Fluent method to attach your Model
        public FluentApiBuilder WithData<TRequest>(TRequest data)
        {
            if (data == null)
            {
                return this;
            }

            var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
            var jsonString = JsonSerializer.Serialize(data, options);
            var dataDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);

            if (dataDict != null)
            {
                foreach (var kvp in dataDict)
                {
                    _payload[kvp.Key] = kvp.Value.ToString();
                }
            }

            return this;
        }

        // 1. Method to add a single header
        public FluentApiBuilder WithHeader(string name, string value)
        {
            // Using the indexer [] ensures that if the header already exists, it updates it.
            // If you used .Add(), it would throw an error on duplicates.
            _headers[name] = value;

            // Returning 'this' is the magic that makes it fluent
            return this;
        }

        // 2. Method to add multiple headers from another dictionary
        public FluentApiBuilder WithHeaders(IDictionary<string, string> headersToAdd)
        {
            if (headersToAdd == null)
            {
                return this;
            }

            foreach (var kvp in headersToAdd)
            {
                _headers[kvp.Key] = kvp.Value;
            }

            return this;
        }

        public async Task<string> ExecuteGetAsync()
        {
            return await ExecuteCoreAsync(req => req.SetQueryParams(_payload).GetAsync());
        }

        public async Task<TResponse> ExecuteGetAsync<TResponse>()
        {
            return Deserialize<TResponse>(await ExecuteGetAsync());
        }

        public async Task<string> ExecutePostJsonAsync()
        {
            return await ExecuteCoreAsync(req => req.PostJsonAsync(_payload));
        }

        public async Task<string> ExecutePostUrlEncodedAsync()
        {
            return await ExecuteCoreAsync(req => req.PostUrlEncodedAsync(_payload));
        }

        public async Task<TResponse> ExecutePostJsonAsync<TResponse>()
        {
            return Deserialize<TResponse>(await ExecutePostJsonAsync());
        }

        public async Task<TResponse> ExecutePostUrlEncodedAsync<TResponse>()
        {
            return Deserialize<TResponse>(await ExecutePostUrlEncodedAsync());
        }


        // Centralized Flurl pipeline
        private async Task<string> ExecuteCoreAsync(Func<IFlurlRequest, Task<IFlurlResponse>> postAction)
        {
            if (string.IsNullOrEmpty(_path))
            {
                throw new InvalidOperationException("You must SetPath() before executing.");
            }

            var request = _baseUrl
                .WithHeaders(_headers)
                .AppendPathSegment(_path);

            // Execute whichever Flurl method was passed in
            var response = await postAction(request);

            var rawJson = await response.GetStringAsync();

            Debug.Print($"RAW API RESPONSE [{_path}] --------------------------------");
            Debug.Print(rawJson);

            return rawJson;
        }

        // Centralized JSON Parsing
        private TResponse Deserialize<TResponse>(string rawJson)
        {
            try
            {
                return JsonConvert.DeserializeObject<TResponse>(rawJson);
            }
            catch (JsonException ex)
            {
                throw new Exception("JSON Parsing Failed: " + ex.Message, ex);
            }
        }
    }
}