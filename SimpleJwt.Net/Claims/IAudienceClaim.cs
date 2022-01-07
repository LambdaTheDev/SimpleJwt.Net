namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // Audience claim, generic argument specifies type for audience identifier type
    public interface IAudienceClaim<T>
    {
        T Aud { get; set; }
    }
}