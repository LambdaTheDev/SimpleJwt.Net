namespace LambdaTheDev.SimpleJwt.Net
{
    // Enum that says if JWT validation was successful, and if not - why
    public enum JwtValidationResult
    {
        Success = 0,
        
        InvalidToken,
        InvalidSignature,
        
        IssuedInFuture,
        UsedTooEarly,
        Expired,
    }
}