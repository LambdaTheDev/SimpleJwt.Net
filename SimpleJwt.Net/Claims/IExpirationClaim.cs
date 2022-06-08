using System;

namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // Expiration (EXP) claim implementation
    public interface IExpirationClaim
    {
        DateTimeOffset Exp { get; set; }
    }
}