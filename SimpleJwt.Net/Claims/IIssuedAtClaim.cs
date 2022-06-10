namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // IssuedAt (IAT) claim implementation
    public interface IIssuedAtClaim
    {
        long Iat { get; set; }
    }
}