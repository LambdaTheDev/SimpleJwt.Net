using System;

namespace LambdaTheDev.SimpleJwt.Net.Algorithms
{
    // Algorithm implementation for SimpleJWT
    public interface IJwtTokenAlgorithm
    {
        string Alg { get; }
        ArraySegment<byte> ComputeSignature(ArraySegment<byte> data);
    }
}