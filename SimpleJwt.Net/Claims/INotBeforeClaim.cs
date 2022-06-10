namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // NotBefore (NBF) claim implementation
    public interface INotBeforeClaim
    {
        long Nbf { get; set; }
    }
}