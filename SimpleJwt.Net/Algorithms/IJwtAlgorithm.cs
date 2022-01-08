using System;

namespace LambdaTheDev.SimpleJwt.Net.Algorithms
{
    // Base interface for all hashing/encryption algorithms used in SimpleJwt
    public interface IJwtAlgorithm
    {
        string Name { get; } // Name included in header
        byte[] Hash(ArraySegment<byte> data); // Hashing function wrapper
    }
}