using System;
using Cysharp.Text;

namespace LambdaTheDev.SimpleJwt.Net.Utils
{
    // Helper class for string operations
    internal static class StringUtils
    {
        private const int InitialBase64BufferCapacity = 1024;
        
        // Buffer used to store chars for .NET standard 2.0 non-alloc TokenPart Base64 decode
        [ThreadStatic] private static char[] _base64TempBuffer;
        
        // Returns ArraySegment with UTF-8 bytes of provided string
        public static ArraySegment<byte> GetUtf8BytesNonAlloc(ref Utf8ValueStringBuilder builder, string str, bool clearBuilder = true)
        {
            if(clearBuilder) builder.Clear();
            builder.Append(str);
            return builder.AsArraySegment();
        }

        // Returns ArraySegment with UTF-8 bytes of provided chars
        public static ArraySegment<byte> GetUtf8BytesNonAlloc(ref Utf8ValueStringBuilder builder, ArraySegment<char> chars, bool clearBuilder = true)
        {
            if(clearBuilder) builder.Clear();
            builder.Append(chars);
            return builder.AsArraySegment();
        }
        
        // Gets Base64 bytes out of TokenPart
        public static ArraySegment<byte> FrameworkSafeGetBase64BytesFromTokenPart(JwtTokenOptions options, TokenPart part)
        {
#pragma warning disable 162
#if NETSTANDARD2_1_OR_GREATER
            ReadOnlySpan<char> tokenSpan = part.Target.AsSpan(part.Offset, part.Count);
            return options.Encoder.FromBaseNonAlloc(tokenSpan);
#endif
            // Write token part data to a buffer
            EnsureBase64BufferSize(part.Count);
            for (int i = 0; i < part.Count; i++)
                _base64TempBuffer[i] = part.Target[part.Offset + i];
            
            // Get Base64
            return options.Encoder.FromBaseNonAlloc(new ArraySegment<char>(_base64TempBuffer, 0, part.Count));
#pragma warning restore
        }

        // Checks if base64 buffer is long enough
        private static void EnsureBase64BufferSize(int requiredLength)
        {
            if (_base64TempBuffer == null)
            {
                if (requiredLength > InitialBase64BufferCapacity)
                    _base64TempBuffer = new char[GetNextPowerOfTwo(requiredLength)];
                else _base64TempBuffer = new char[InitialBase64BufferCapacity];
                return;
            }

            if (_base64TempBuffer.Length < requiredLength) return;
            _base64TempBuffer = new char[GetNextPowerOfTwo(requiredLength)];
        }

        // Returns first greater power of 2 than N
        private static int GetNextPowerOfTwo(int n)
        {
            int result = 1;
            while (result < n)
                result *= 2;

            return result;
        }
    }
}