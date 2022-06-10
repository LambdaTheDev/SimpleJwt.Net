using System;
using System.Runtime.CompilerServices;

namespace LambdaTheDev.SimpleJwt.Net.Extensions
{
    // ArraySegment<char> extensions
    internal static class CharArraySegmentExtensions
    {
        // Constructs string out of char ArraySegment
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MakeString(this ArraySegment<char> chars)
        {
            if (chars.Array == null) return null;
            return new string(chars.Array, chars.Offset, chars.Count);
        }
    }
}