using System;

namespace LambdaTheDev.SimpleJwt.Net
{
    // Header for JWT token
    [Serializable]
    public readonly struct JwtHeader
    {
        public string Typ { get; } // Token type, always JWT
        public string Alg { get; } // Algorithm used by that JWT instance, set by generator

        
        public JwtHeader(string alg)
        {
            Typ = "JWT";
            Alg = alg;
        }
    }
}