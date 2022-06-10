using System;

namespace LambdaTheDev.SimpleJwt.Net.Algorithms
{
    // Algorithm implementation for SimpleJWT
    public interface IJwtTokenAlgorithm
    {
        // Algorithm identifier, used in JWT header
        string Alg { get; }
        
        // If signature algorithm returns constant size signatures, return their size in bytes. Otherwise, return -1
        int SignatureSize { get; }
        
        // Method that computes signature for provided data
        ArraySegment<byte> ComputeSignature(ArraySegment<byte> data);
    }
}