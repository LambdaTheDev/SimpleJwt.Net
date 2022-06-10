namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // Expiration (EXP) claim implementation
    public interface IExpirationClaim
    {
        long Exp { get; set; }
    }
}