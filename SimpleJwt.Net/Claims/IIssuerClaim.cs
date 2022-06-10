namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // Issuer (ISS) claim implementation
    public interface IIssuerClaim<T>
    {
        T Iss { get; set; }
    }
}