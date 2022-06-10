using System.Runtime.CompilerServices;

namespace LambdaTheDev.SimpleJwt.Net.Extensions
{
    // Extension methods for JwtTokenOptions
    public static class JwtTokenOptionsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Generate<T>(this JwtTokenOptions options, T payload)
        {
            return JwtToken.Generate(options, payload);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JwtValidationResult Validate<T>(this JwtTokenOptions options, string token, out T payload)
        {
            return JwtToken.Validate(options, token, out payload);
        }
    }
}