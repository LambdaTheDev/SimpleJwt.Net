using System;

namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // IssuedAt (IAT) claim implementation
    public interface IIssuedAtClaim
    {
        DateTimeOffset Iat { get; set; }
    }
}