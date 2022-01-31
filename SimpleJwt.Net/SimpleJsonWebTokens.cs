using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using LambdaTheDev.SharpStringUtils;
using LambdaTheDev.SharpStringUtils.Encodings;
using LambdaTheDev.SharpStringUtils.Encodings.Base;
using LambdaTheDev.SharpStringUtils.Extensions;
using LambdaTheDev.SharpStringUtils.Iterator;
using LambdaTheDev.SimpleJwt.Net.Algorithms;
using LambdaTheDev.SimpleJwt.Net.Claims;

namespace LambdaTheDev.SimpleJwt.Net
{
    // Most important class in this library. Allows to generate & validate tokens
    public sealed class SimpleJsonWebTokens
    {
        // Raw UTF8 encoding instance
        private readonly UTF8Encoding _rawEncoding = new UTF8Encoding(false, false);
        
        // Base64 encoder/decoder that does not allocate much
        private readonly Base64EncoderNonAlloc _base64 = new Base64EncoderNonAlloc('-', '_', false);
        
        // StringBuilder, but bitwise & for UTF8 encoding
        private readonly EncodingBuilderNonAlloc _utf8Builder;
        
        // Encoding instance that does not allocate
        private readonly EncodingNonAlloc _utf8Encoding;

        // Algorithm instance used by this SJWT instance
        public IJwtAlgorithm Algorithm { get; }
        
        // Encoded header, internal for testing proposes
        internal string EncodedHeader { get; }
        
        
        public SimpleJsonWebTokens(IJwtAlgorithm algorithm)
        {
            // Set algorithm & encoding wrappers
            // Note: I use single UTF-8 instance, due to it's not used concurrently in single SJWT instance
            Algorithm = algorithm;
            _utf8Builder = new EncodingBuilderNonAlloc(_rawEncoding);
            _utf8Encoding = new EncodingNonAlloc(_rawEncoding);

            // Prepare header & serialize into JSON
            JwtHeader header = new JwtHeader(algorithm.Name);
            string jsonHeader = JsonSerializer.Serialize(header);
            
            // Get bytes & create base64 string
            _utf8Builder.Clear();
            _utf8Builder.Append(new StringSegment(jsonHeader));
            ArraySegment<char> base64Chars = _base64.ToBaseNonAlloc(_utf8Builder.GetBytesNonAlloc());

            // Set base64 string with dot at the end as header
            EncodedHeader = new string(base64Chars.Array, base64Chars.Offset, base64Chars.Count);
        }
        
        // Generates a token where T type is a payload.
        public string Generate<T>(T payload)
        {
            string payloadJson = JsonSerializer.Serialize(payload);
            ArraySegment<byte> payloadJsonUtf8 = _utf8Encoding.GetBytesNonAlloc(payloadJson);
            ArraySegment<char> payloadBase64 = _base64.ToBaseNonAlloc(payloadJsonUtf8);
            
            // Reset builder & append header & Base
            _utf8Builder.Clear();
            _utf8Builder.Append(EncodedHeader, ".");
            _utf8Builder.Append(payloadBase64, ".");
            
            // Create signature & append Base64
            byte[] signature = Algorithm.Hash(_utf8Builder.GetBytesNonAlloc());
            ArraySegment<char> base64Signature = _base64.ToBaseNonAlloc(signature);
            _utf8Builder.Append(base64Signature, ".");
            
            // Generate token from builder
            return _utf8Builder.GetString();
        }

        // QOL wrapper for string token validation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Validate<T>(string token) => Validate<T>(new StringSegment(token));

        // Validates JWT token. Throws InvalidTokenException, if it's invalid
        public T Validate<T>(StringSegment tokenSegment)
        {
            try
            {
                // todo: Reduce allocation by allowing StringSegment in StringSplitter + rework string splitter
                string token = tokenSegment.ToString();

                // Prepare default values
                StringSegment header = default, payload = default, signature = default;

                int iterations = 0;
                foreach (StringSegment segment in new StringSplitterNonAlloc(token, '.'))
                {
                    // Assign each part to cached segment above
                    switch (iterations)
                    {
                        case 0:
                            header = segment;
                            break;

                        case 1:
                            payload = segment;
                            break;

                        case 2:
                            signature = segment;
                            break;

                        // If more then 2 (3), throw InvalidToken
                        default:
                            throw new InvalidTokenException(JwtFailureCause.InvalidFormat);
                    }

                    iterations++;
                }

                // Assert that there is not too much/less segments
                if (iterations != 3)
                    throw new InvalidTokenException(JwtFailureCause.InvalidFormat);

                // Validate header (easiest - just ensure that it matches EncodedHeader
                if (!header.Equals(EncodedHeader))
                    throw new InvalidTokenException(JwtFailureCause.InvalidHeader);

                // Prepare signature
                _utf8Builder.Clear();
                _utf8Builder.Append(header, ".");
                _utf8Builder.Append(payload, ".");

                // Compute & validate signature
                byte[] computedHash = Algorithm.Hash(_utf8Builder.GetBytesNonAlloc());
                ArraySegment<char> signatureBase64Chars = _base64.ToBaseNonAlloc(computedHash);

                // Validate signature. TODO: Add ArrSegment<char> support to StringSegment
                if(signatureBase64Chars.Count != signature.Count)
                    throw new InvalidTokenException(JwtFailureCause.InvalidSignature);
                
                // Iterate through chars to ensure strings match.
                for(int i = 0; i < signature.Count; i++)
                    if(signatureBase64Chars.Array[signatureBase64Chars.Offset + i] != signature.OriginalString[signature.Offset + i])
                        throw new InvalidTokenException(JwtFailureCause.InvalidSignature);

                // Decode payload & deserialize it from JSON
                ArraySegment<byte> payloadJsonBytes = _base64.FromBaseNonAlloc(payload);
                string payloadJson = _utf8Encoding.GetString(payloadJsonBytes);

                // Pass payload to claims validator 
                T deserializedPayload = JsonSerializer.Deserialize<T>(payloadJson);
                EnsureClaimsAreValid(deserializedPayload);

                // If everything is alright - return payload
                return deserializedPayload;
            }
            catch (InvalidTokenException) { throw; }
            catch (Exception)
            {
                throw new InvalidTokenException(JwtFailureCause.Unknown);
            }
        }

        // Gets values from Claims interfaces & validates them
        private void EnsureClaimsAreValid<T>(T payload)
        {
            // If payload has expiration check, compare it against now (UTC)
            if (payload is IExpirationClaim exp)
            {
                if(exp.Exp <= DateTime.UtcNow)
                    throw new InvalidTokenException(JwtFailureCause.Expired);
            }

            // If payload has issued at check, ensure it's not issued in future somehow
            if (payload is IIssuedAtClaim iat)
            {
                if(iat.Iat > DateTime.UtcNow)
                    throw new InvalidTokenException(JwtFailureCause.IssuedInFuture);
            }

            // If payload has not before check, ensure that if token is used in a proper time
            if (payload is INotBeforeClaim nbf)
            {
                if(nbf.Nbf > DateTime.UtcNow)
                    throw new InvalidTokenException(JwtFailureCause.UsedTooEarly);
            }
        }
    }
}