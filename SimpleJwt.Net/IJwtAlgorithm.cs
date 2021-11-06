namespace SimpleJwt.Net
{
    // Algorithm implementation
    public interface IJwtAlgorithm
    {
        public string Code { get; }
        public string Hash(string payload);
    }
}