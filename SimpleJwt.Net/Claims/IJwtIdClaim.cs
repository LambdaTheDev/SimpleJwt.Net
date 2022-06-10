namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // JWT ID (JTI) claim implementation
    public interface IJwtIdClaim<T>
    {
        T Jti { get; set; }
    }
}