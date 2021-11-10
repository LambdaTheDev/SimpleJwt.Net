using System;

namespace SimpleJwt.Net
{
    // Algorithm implementation for SimpleJwt.Net
    // IMPORTANT: RIGHT NOW ONLY FIXED-SIZE 
    public interface IJwtAlgorithm
    {
        int HashLength { get; } // How much bytes does hash take
        string Code { get; } // Algorithm code used in header
    }
}