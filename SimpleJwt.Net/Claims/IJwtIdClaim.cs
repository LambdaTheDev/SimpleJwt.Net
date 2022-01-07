namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // JWT Id claim, shows ID of a JWT. I used int as an identifier. If there is a need
    //  to use longs/ulongs, please contact me
    public interface IJwtIdClaim
    {
        int Jti { get; set; }
    }
}