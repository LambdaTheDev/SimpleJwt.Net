using System;

namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // Issued at claim, shows when JWT token was issued. Auto validated by library
    public interface IIssuedAtClaim
    {
        DateTime Iat { get; set; }
    }
}