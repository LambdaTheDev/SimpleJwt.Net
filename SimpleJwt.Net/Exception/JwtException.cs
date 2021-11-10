namespace SimpleJwt.Net.Exception
{
    // Exception thrown when JWT validation is invalid.
    public class JwtException : System.Exception
    {
        public JwtFailureCause Cause { get; } // Failure cause

        public JwtException(JwtFailureCause cause)
        {
            Cause = cause;
        }
    }
}