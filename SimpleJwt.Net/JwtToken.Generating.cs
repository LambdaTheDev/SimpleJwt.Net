using System;
using System.Text.Json;
using Cysharp.Text;
using LambdaTheDev.SimpleJwt.Net.Utils;

namespace LambdaTheDev.SimpleJwt.Net
{
    // Class that generates JWT token basing on JwtTokenOptions & custom payload
    public static partial class JwtToken
    {
        public static string Generate<T>(JwtTokenOptions options, T payload)
        {
            // Prepare StringBuilder
            string token = null;
            Utf8ValueStringBuilder builder = ZString.CreateUtf8StringBuilder();
            ValueUsing<Utf8ValueStringBuilder>.Use(ref builder, (ref Utf8ValueStringBuilder usedBuilder) =>
            {
                // Generate payload data
                string payloadJson = JsonSerializer.Serialize(payload, options.SerializerOptions);
                ArraySegment<byte> payloadJsonBytes = StringUtils.GetUtf8BytesNonAlloc(ref usedBuilder, payloadJson);
                ArraySegment<char> payloadJsonBase64 = options.Encoder.ToBaseNonAlloc(payloadJsonBytes);
                
                // Combine data to compute signature
                usedBuilder.Clear();
                usedBuilder.Append(options.PreComputedHeader);
                usedBuilder.Append('.');
                usedBuilder.Append(payloadJsonBase64);
                ArraySegment<byte> combinedBytes = usedBuilder.AsArraySegment();
                
                // Compute signature
                ArraySegment<byte> signature = options.Algorithm.ComputeSignature(combinedBytes);
                ArraySegment<char> signatureBase64 = options.Encoder.ToBaseNonAlloc(signature);

                // Append signature to builder
                usedBuilder.Append('.');
                usedBuilder.Append(signatureBase64);
                
                // Construct token
                token = usedBuilder.ToString();
                usedBuilder.Clear();
            });
            
            // Return generated token
            return token;
        }
    }
}