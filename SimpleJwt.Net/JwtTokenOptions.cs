using System;
using System.Collections.Generic;
using System.Text.Json;
using Cysharp.Text;
using LambdaTheDev.Exyll.Base64EncoderNonAlloc;
using LambdaTheDev.SimpleJwt.Net.Algorithms;
using LambdaTheDev.SimpleJwt.Net.AutoValidators;
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

        // Attached AutoValidators
        public readonly List<JwtAutoValidator> AutoValidators = new List<JwtAutoValidator>();

        // Pre-computed header, to reduce allocations while generating tokens
        internal string PreComputedHeader { get; }
        
        
        public JwtTokenOptions(IJwtTokenAlgorithm algorithm, JsonSerializerOptions serializerOptions, bool includeDatesAutoValidator = true)
        {
            Algorithm = algorithm;
            SerializerOptions = serializerOptions;
            PreComputedHeader = ComputeHeader();
            
            if(includeDatesAutoValidator)
                AutoValidators.Add(new DatesAutoValidator());
        }

        // Expansive method used to pre-compute headers
        private string ComputeHeader()
        {
            // Prepare data
            TokenHeader header = new TokenHeader(Algorithm.Alg);
            string headerJson = JsonSerializer.Serialize(header, SerializerOptions);
            
            // Process data
            string computedHeader = null;
            Utf8ValueStringBuilder builder = ZString.CreateUtf8StringBuilder();
            ValueUsing<Utf8ValueStringBuilder>.Use(ref builder, (ref Utf8ValueStringBuilder usedBuilder) =>
            {
                ArraySegment<byte> headerJsonBytes = StringUtils.GetUtf8BytesNonAlloc(ref usedBuilder, headerJson);
                ArraySegment<char> headerJsonBytesBase64 = Encoder.ToBaseNonAlloc(headerJsonBytes);
                computedHeader = headerJsonBytesBase64.MakeString();
            });

            return computedHeader;
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