using System;
using LambdaTheDev.SimpleJwt.Net.Algorithms;

namespace LambdaTheDev.SimpleJwt.Net.Extensions
{
    // Extension methods for JwtTokenAlgorithms
    public static class JwtTokenAlgorithmExtensions
    {
        // Returns signature base64 length, or -1 if it's impossible to determine
        public static int GetSignatureChars(this IJwtTokenAlgorithm algorithm)
        {
            if (algorithm == null) return -1;
            if (algorithm.SignatureSize == -1) return -1;
            
            int padding = algorithm.SignatureSize % 3;
            if (padding > 0)
                padding = 3 - padding;
            int blocks = (algorithm.SignatureSize - 1) / 3 + 1;

            int l = blocks * 4;
            return l - padding;
        }
    }
}