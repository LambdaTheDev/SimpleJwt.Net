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
            Utf8ValueStringBuilder builder = ZString.CreateUtf8StringBuilder();
            try
            {
                // Prepare StringBuilder
                string token = null;
               
                // Generate payload data
                string payloadJson = JsonSerializer.Serialize(payload, options.SerializerOptions);
                ArraySegment<byte> payloadJsonBytes = StringUtils.GetUtf8BytesNonAlloc(ref builder, payloadJson);
                ArraySegment<char> payloadJsonBase64 = options.Encoder.ToBaseNonAlloc(payloadJsonBytes);
                
                // Combine data to compute signature
                builder.Clear();
                builder.Append(options.PreComputedHeader);
                builder.Append('.');
                builder.AppendCharArraySegment(payloadJsonBase64);
                ArraySegment<byte> combinedBytes = builder.AsArraySegment();
                
                // Compute signature
                ArraySegment<byte> signature = options.Algorithm.ComputeSignature(combinedBytes);
                ArraySegment<char> signatureBase64 = options.Encoder.ToBaseNonAlloc(signature);

                // Append signature to builder
                builder.Append('.');
                builder.AppendCharArraySegment(signatureBase64);
                
                // Construct token
                token = builder.ToString();
                builder.Clear();

                // Return generated token
                return token;
            }
            finally
            {
                builder.Dispose();
            }
        }
    }
}