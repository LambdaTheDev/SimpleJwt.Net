namespace SimpleJwt.Net
{
    // Algorithm implementation for SimpleJwt.Net
    // IMPORTANT: RIGHT NOW ONLY FIXED-SIZE 
    public interface IJwtAlgorithm
    {
        string Code { get; } // Algorithm code used in header
        string Hash(string input); // Computes a hash for provided string
    }
}