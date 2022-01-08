namespace LambdaTheDev.SimpleJwt.Net.StringUtils
{
    // ArraySegment equivalent, but for string
    public readonly struct StringSegment
    {
        public static readonly StringSegment Null = new StringSegment(null, -1, 0);
        public static readonly StringSegment Empty = new StringSegment(string.Empty, 0, -1);
        
        public readonly string OriginalString; // Original string
        public readonly int Offset; // Start character index
        public readonly int Count; // Count of characters from index


        public StringSegment(string originalString, int offset, int count)
        {
            OriginalString = originalString;
            Offset = offset;
            Count = count;

            if (originalString == null)
                Offset = -1;
            else if (originalString == string.Empty)
                Count = -1;
        }

        public bool IsNull => Offset == -1;
        public bool IsEmpty => Count == -1;
        public bool IsNullOrEmpty => Offset == -1 || Count == -1;


        public override string ToString()
        {
            return OriginalString.Substring(Offset, Count);
        }
    }
}