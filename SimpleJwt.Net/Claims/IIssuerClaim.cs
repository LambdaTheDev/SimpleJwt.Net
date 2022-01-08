namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // Issuer claim, shows who issued a token, with a generic issuer identifier type
    public interface IIssuerClaim<T>
    {
        T Iss { get; set; }
    }
}