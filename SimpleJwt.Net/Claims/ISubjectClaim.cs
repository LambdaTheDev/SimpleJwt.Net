namespace LambdaTheDev.SimpleJwt.Net.Claims
{
    // Subject claim, represents a entity who is this JWT for, identified with a generic type
    public interface ISubjectClaim<T>
    {
        T Sub { get; set; }
    }
}