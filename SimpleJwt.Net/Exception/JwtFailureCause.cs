namespace SimpleJwt.Net.Exception
{
    public enum JwtFailureCause : byte
    {
        None = 0,
        InvalidToken, // Token is just invalid
        InvalidAlgorithm, // Algorithm in incoming token isn't same as a validation token
        InvalidType, // This is not a JWT token
        InvalidSignature, // Signature isn't valid
        InvalidIssuer, // Issuer is not the same
        TooNew, // Token is too new (issued in future?)
        Expired, // Token expired
    }
}