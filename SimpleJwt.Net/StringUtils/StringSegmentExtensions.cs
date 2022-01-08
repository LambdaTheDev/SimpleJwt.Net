using System;
using System.Text.Json;

namespace LambdaTheDev.SimpleJwt.Net.StringUtils
{
    // Extension methods for StringSegment
    public static class StringSegmentExtensions
    {
#if NET
        public static T DeserializeJson<T>(this StringSegment segment, EncodingWrapper wrapper)
#else
        public static T DeserializeJson<T>(this StringSegment segment)
#endif
        {
#if NET
            // .NET has Span support...

            if(wrapper.Encoding.BodyName != "utf-8")
                throw new ArgumentException("You must provide EncodingWrapper for UTF8!");
            
            wrapper.Clear();
            wrapper.Append(segment);

            ArraySegment<byte> bytes = wrapper.ToReusableBuffer();
            
            ReadOnlySpan<byte> utf8Bytes = new ReadOnlySpan<byte>(bytes.Array, bytes.Offset, bytes.Count);
            return JsonSerializer.Deserialize<T>(utf8Bytes);
#endif
            
            // todo: Try to figure out a non/least alloc solution for Mono framework...
            return JsonSerializer.Deserialize<T>(segment.ToString());
        }
    }
}