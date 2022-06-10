namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // Subject (SUB) claim implementation
    public interface ISubjectClaim<T>
    {
        T Sub { get; set; }
    }
}