using System;
using Microsoft.Extensions.Configuration;

namespace AcmeIntegreatedAssessement.Infrastructure.Configuration;

public interface IConnectionStringResolver
{
    string? GetConnectionString(string name);
}

public class ConnectionStringResolver : IConnectionStringResolver
{
    private readonly IConfiguration _configuration;

    public ConnectionStringResolver(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string? GetConnectionString(string name)
    {
        var environmentValue = Environment.GetEnvironmentVariable($"ConnectionStrings__{name}");
        if (!string.IsNullOrWhiteSpace(environmentValue))
        {
            return environmentValue;
        }

        return _configuration.GetConnectionString(name);
    }
}
