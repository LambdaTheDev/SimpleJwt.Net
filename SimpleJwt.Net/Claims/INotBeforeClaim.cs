using System;

namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // Not before claim, it represents a date from what JWT token can be used.
    // Auto validated by library.
    public interface INotBeforeClaim
    {
        DateTime Nbf { get; set; }
    }
}