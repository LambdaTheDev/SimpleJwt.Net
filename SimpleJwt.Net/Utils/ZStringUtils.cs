using System;
using System.Runtime.CompilerServices;
using Cysharp.Text;

namespace LambdaTheDev.SimpleJwt.Net.Utils
{
    // Utility methods related to ZStringUtils class
    internal static class ZStringUtils
    {
        // Casually appends TokenPart to StringBuilder
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AppendCharArraySegment(this ref Utf8ValueStringBuilder builder, ArraySegment<char> value)
        {
            ReadOnlySpan<char> chars = value;
            builder.Append(chars);
        }
    }
}