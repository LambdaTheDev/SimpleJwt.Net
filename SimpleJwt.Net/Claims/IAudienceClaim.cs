namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // Audience (AUD) claim implementation
    public interface IAudienceClaim<T>
    {
        T Aud { get; set; }
    }
}