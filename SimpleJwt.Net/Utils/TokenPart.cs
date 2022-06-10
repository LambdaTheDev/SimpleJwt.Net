namespace LambdaTheDev.SimpleJwt.Net.Utils
{
    // Represents string part
    internal readonly struct TokenPart
    {
        public readonly string Target;
        public readonly int Offset;
        public readonly int Count;

        
        public TokenPart(string target, int offset, int count)
        {
            Target = target;
            Offset = offset;
            Count = count;
        }
    }
}