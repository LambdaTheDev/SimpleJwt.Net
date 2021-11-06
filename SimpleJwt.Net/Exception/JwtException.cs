namespace SimpleJwt.Net.Exception
{
    public class JwtException : System.Exception
    {
        public JwtFailureCause Cause { get; }

        public JwtException(JwtFailureCause cause)
        {
            Cause = cause;
        }
    }
}