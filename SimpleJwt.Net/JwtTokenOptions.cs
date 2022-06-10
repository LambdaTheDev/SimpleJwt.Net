using System;
using System.Text.Json;
using Cysharp.Text;
using LambdaTheDev.Exyll.Base64EncoderNonAlloc;
using LambdaTheDev.SimpleJwt.Net.Algorithms;
using LambdaTheDev.SimpleJwt.Net.Extensions;
using LambdaTheDev.SimpleJwt.Net.Utils;

namespace LambdaTheDev.SimpleJwt.Net
{
    // JWT token options container
    public sealed class JwtTokenOptions
    {
        // Algorithm used for tokens generated with those options
        public IJwtTokenAlgorithm Algorithm { get; }
        
        // JSON serializer options
        public JsonSerializerOptions SerializerOptions { get; }
        
        // Locked Base64 encoder (note: I will try to make a better solution)
        private readonly BaseEncoder _encoder = Base64Encoder.UrlEncoding.Copy();
        public BaseEncoder Encoder
        {
            get { lock (_encoder) { return _encoder; } }
        }

        // Pre-computed header, to reduce allocations while generating tokens
        internal string PreComputedHeader { get; }
        
        
        public JwtTokenOptions(IJwtTokenAlgorithm algorithm, JsonSerializerOptions serializerOptions)
        {
            Algorithm = algorithm;
            SerializerOptions = serializerOptions;
            PreComputedHeader = ComputeHeader();
        }

        // Expansive method used to pre-compute headers
        private string ComputeHeader()
        {
            // Prepare data
            TokenHeader header = new TokenHeader(Algorithm.Alg);
            string headerJson = JsonSerializer.Serialize(header, SerializerOptions);
            
            // Process data
            Utf8ValueStringBuilder builder = ZString.CreateUtf8StringBuilder();
            try
            {
                ArraySegment<byte> headerJsonBytes = StringUtils.GetUtf8BytesNonAlloc(ref builder, headerJson);
                ArraySegment<char> headerJsonBytesBase64 = Encoder.ToBaseNonAlloc(headerJsonBytes);
                return headerJsonBytesBase64.MakeString();
            }
            finally
            {
                builder.Dispose();
            }
        }
    }
    
    // JWT token header structure
    [Serializable]
    internal sealed class TokenHeader
    {
        public string Typ => "JWT";
        public string Alg { get; }

        public TokenHeader(string alg) { Alg = alg; }
    }
}