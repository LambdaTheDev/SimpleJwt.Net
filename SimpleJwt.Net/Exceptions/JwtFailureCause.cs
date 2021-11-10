namespace SimpleJwt.Net.Exceptions
{
    public enum JwtFailureCause : byte
    {
        // General failures
        None = 0, // Default value
        TokenInvalid, // Token is invalid, not in xxx.yyy.zzz template
        AlgorithmInvalid, // Validator's algorithm is different than token's one
        SignatureInvalid, // Token has invalid signature
        
        // Generation failures
        TooShortExpiryTime, // Token expires before it's generated
        
        // Basic payload failure causes
        IssuerClaimFailed, // Token's and validators issuer does not match
        IssuedAtClaimFailed, // Attempted to validate a token that has been issued... in future?
        ExpirationClaimFailed, // Attempted to validate a token that is expired
        
        // Advanced payload failure causes
        AudienceClainFailed, 
        NotBeforeClaimFailed,
        JwtIdClaimFailed,
    }
}