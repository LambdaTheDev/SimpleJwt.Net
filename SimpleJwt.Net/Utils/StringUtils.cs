using System;
using Cysharp.Text;

namespace LambdaTheDev.SimpleJwt.Net.Utils
{
    // Helper class for string operations
    internal static class StringUtils
    {
        // Returns ArraySegment with UTF-8 bytes of provided string
        public static ArraySegment<byte> GetUtf8BytesNonAlloc(ref Utf8ValueStringBuilder builder, string str, bool clearBuilder = true)
        {
            if(clearBuilder) builder.Clear();
            builder.Append(str);
            return builder.AsArraySegment();
        }
    }
}