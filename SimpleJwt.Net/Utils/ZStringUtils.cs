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
        public static void AppendTokenPart(this ref Utf8ValueStringBuilder builder, TokenPart value)
        {
            ReadOnlySpan<char> chars = value.Target.AsSpan(value.Offset, value.Count);
            builder.Append(chars);
        }
        
        // Casually appends TokenPart to StringBuilder
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AppendCharArraySegment(this ref Utf8ValueStringBuilder builder, ArraySegment<char> value)
        {
            ReadOnlySpan<char> chars = value;
            builder.Append(chars);
        }
    }
}