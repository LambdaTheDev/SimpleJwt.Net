using System;

namespace SimpleJwt.Net
{
    // Basic JWT clauses, suitable for most users
    public interface IJwtPayload
    {
        public string Iss { get; set; } // Token issuer
        public string Sub { get; set; } // Token subject, for example user ID
        public DateTime Iat { get; set; } // Date when this token was issued
        public DateTime Exp { get; set; } // Date when this token expires
    }
}