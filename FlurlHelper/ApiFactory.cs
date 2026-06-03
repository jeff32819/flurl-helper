namespace FlurlHelper;

public interface IApiFactory
{
    FluentApiBuilder CreateClient();          // For public calls (Login, Register)
    FluentApiBuilder CreateAuthClient();      // For secure calls
}

public class ApiFactory : IApiFactory
{
    private readonly string _baseUrl;
    private readonly string _token;

    public ApiFactory(string baseUrl, string token = null)
    {
        _baseUrl = baseUrl;
        _token = token;
    }

    // 1. Spawns a clean builder with NO authentication
    public FluentApiBuilder CreateClient()
    {
        return new FluentApiBuilder(_baseUrl);
    }

    // 2. Spawns a builder with the Bearer token attached
    public FluentApiBuilder CreateAuthClient()
    {
        var builder = new FluentApiBuilder(_baseUrl);

        if (!string.IsNullOrWhiteSpace(_token))
        {
            builder.WithBearerToken(_token);
        }

        return builder;
    }
}