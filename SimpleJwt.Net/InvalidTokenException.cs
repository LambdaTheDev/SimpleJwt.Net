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
        None = 0,
    }
}