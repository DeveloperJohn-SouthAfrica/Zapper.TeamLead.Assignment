using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace AcmeIntegreatedAssessement.Api.Services;

public static class AcmeCallbackAuthentication
{
    public static bool IsValidBasicCredentials(HttpRequest request, string? expectedUsername, string? expectedPassword)
    {
        var header = request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(expectedUsername) ||
            string.IsNullOrWhiteSpace(expectedPassword) ||
            header.Length > 4096 ||
            !header.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        try
        {
            var credentialBytes = Convert.FromBase64String(header[6..].Trim());
            var separator = Array.IndexOf(credentialBytes, (byte)':');
            if (separator <= 0)
            {
                return false;
            }

            var actualUsername = credentialBytes.AsSpan(0, separator);
            var actualPassword = credentialBytes.AsSpan(separator + 1);
            return FixedTimeEquals(actualUsername, Encoding.UTF8.GetBytes(expectedUsername)) &&
                   FixedTimeEquals(actualPassword, Encoding.UTF8.GetBytes(expectedPassword));
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static bool IsValidSignature(ReadOnlySpan<byte> body, string? suppliedSignature, string? secret)
    {
        if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(suppliedSignature))
        {
            return false;
        }

        try
        {
            var actual = Convert.FromBase64String(suppliedSignature);
            var expected = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), body);
            return FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool FixedTimeEquals(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right) =>
        left.Length == right.Length && CryptographicOperations.FixedTimeEquals(left, right);
}
