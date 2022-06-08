using System;

namespace LambdaTheDev.SimpleJwt.Net
{
    // Class that validates & returns JWT token basing on JwtTokenOptions & string token
    public static partial class JwtToken
    {
        public static JwtValidationResult Validate<T>(JwtTokenOptions options, string token, out T payload)
        {
            throw new NotImplementedException();
        }
    }
}