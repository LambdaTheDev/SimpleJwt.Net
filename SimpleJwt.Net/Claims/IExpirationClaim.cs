using System;

namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // Expiration claim, shows when JWT expires. Auto validated by library
    public interface IExpirationClaim
    {
        DateTime Exp { get; set; }
    }
}