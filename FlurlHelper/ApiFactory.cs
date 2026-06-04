namespace FlurlHelper;

public interface IApiFactory
{
    FluentApiBuilder CreateClient();          // For public calls (Login, Register)
    FluentApiBuilder CreateAuthClient();      // For secure calls
}

public class ApiFactory(string baseUrl, string token = null) : IApiFactory
{
    // 1. Spawns a clean builder with NO authentication
    public FluentApiBuilder CreateClient()
    {
        return new FluentApiBuilder(baseUrl);
    }

    // 2. Spawns a builder with the Bearer token attached
    public FluentApiBuilder CreateAuthClient()
    {
        var builder = new FluentApiBuilder(baseUrl);

        if (!string.IsNullOrWhiteSpace(token))
        {
            builder.WithBearerToken(token);
        }

        return builder;
    }
}