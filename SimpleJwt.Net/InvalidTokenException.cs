using System;

namespace LambdaTheDev.SimpleJwt.Net
{
    // Exception thrown when JWT token was invalid
    public sealed class InvalidTokenException : Exception
    {
        public JwtFailureCause Cause { get; }

        public InvalidTokenException(JwtFailureCause cause) : base("JWT validation failed! Cause: " + cause + '!')
        {
            Cause = cause;
        }
    }

    // Helper enum for applications, to get a cause
    public enum JwtFailureCause
    {
        None = 0, // No failure
        
        InvalidFormat, // Token is null, or not in JWT format
        InvalidHeader, // Header is invalid 
        InvalidSignature, // Signature is invalid, token was prob. modified
        IssuedInFuture, // Issued At claim says that token was issued in future
        UsedTooEarly, // Not Before claim failed
        Expired, // Expiration claim failed,
        
        Unknown, // Token failed for unknown reason, most likely library error
    }
}