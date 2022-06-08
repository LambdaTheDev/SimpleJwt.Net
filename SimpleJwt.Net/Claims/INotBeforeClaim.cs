using System;

namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // NotBefore (NBF) claim implementation
    public interface INotBeforeClaim
    {
        DateTimeOffset Nbf { get; set; }
    }
}