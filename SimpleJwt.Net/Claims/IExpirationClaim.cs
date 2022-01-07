using System;

namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // Expiration claim, shows when JWT expires
    public interface IExpirationClaim
    {
        DateTime Exp { get; set; }
    }
}