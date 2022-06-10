using System;
using System.Text.Json;
using Cysharp.Text;
using LambdaTheDev.SimpleJwt.Net.Claims;
using LambdaTheDev.SimpleJwt.Net.Utils;

namespace LambdaTheDev.SimpleJwt.Net
{
    // Class that validates & returns JWT token basing on JwtTokenOptions & string token
    public static partial class JwtToken
    {
        public static JwtValidationResult Validate<T>(JwtTokenOptions options, string token, out T payload)
        {
            payload = default;
            
            // Ensure that token is in valid format
            bool splitSuccessfully = TrySplitToken(options, token, out TokenParts parts);
            if (!splitSuccessfully) return JwtValidationResult.InvalidToken;
            
            // Prepare data for validation
            TokenPart rawHeader = new TokenPart(token, parts.HeaderOffset, parts.HeaderCount);
            TokenPart rawPayload = new TokenPart(token, parts.PayloadOffset, parts.PayloadCount);
            TokenPart rawSignature = new TokenPart(token, parts.SignatureOffset, parts.SignatureCount);

            Utf8ValueStringBuilder builder = ZString.CreateUtf8StringBuilder();
            try
            {
                // Start validating token
                builder.Clear();
                builder.AppendTokenPart(rawHeader);
                builder.Append('.');
                builder.AppendTokenPart(rawPayload);
                
                // Re-compute signature
                ArraySegment<byte> combinedBytes = builder.AsArraySegment();
                ArraySegment<byte> signature = options.Algorithm.ComputeSignature(combinedBytes);
                ArraySegment<char> signatureBase64 = options.Encoder.ToBaseNonAlloc(signature);
                
                // Validate signature
                bool signatureValid = ValidateSignature(signatureBase64, rawSignature);
                if (!signatureValid) return JwtValidationResult.InvalidSignature;

                // Deserialize payload
                // IMPORTANT NOTE: JsonSerializer.Deserialize method accepts UTF-8 bytes as an deserialization argument. I am gonna use it, but I'm unsure if Unity supports it.
                //  If users will need Unity support (below 2021.2), I will figure out something
                ArraySegment<byte> payloadJsonBytes = StringUtils.FrameworkSafeGetBase64BytesFromTokenPart(options, rawPayload);
                var deserializedPayload = JsonSerializer.Deserialize<T>(payloadJsonBytes, options.SerializerOptions);

                // Clear builder buffer
                builder.Clear();
                
                // Validate date-related claims
                JwtValidationResult result = ValidateClaims(payload);
                if (result == JwtValidationResult.Success) payload = deserializedPayload;
                return result;
            }
            finally
            {
                builder.Dispose();
            }
        }

        private static bool TrySplitToken(JwtTokenOptions options, string token, out TokenParts parts)
        {
            // Ensure that token is long enough to hold data
            int requiredTokenLength = options.PreComputedHeader.Length;
            if (options.Algorithm.SignatureSize != -1) requiredTokenLength += options.Algorithm.SignatureSize;
            requiredTokenLength += 2 + 2; // Add 2 dots and at least some 2 data chars

            if (token.Length < requiredTokenLength)
            {
                parts = default;
                return false;
            }
            
            // Ensure that header is where it should be & set values
            if (token[options.PreComputedHeader.Length] != '.')
            {
                parts = default;
                return false;
            }

            int headerOffset = 0, headerCount = options.PreComputedHeader.Length;
            
            // Now check for signature. If it's constant size, do it as above, otherwise reverse for & find out first dot
            int signatureOffset = -1, signatureCount = -1;
            
            if (options.Algorithm.SignatureSize != -1)
            {
                if (token[token.Length - options.Algorithm.SignatureSize - 1] != '.')
                {
                    parts = default;
                    return false;
                }

                signatureOffset = token.Length - options.Algorithm.SignatureSize;
                signatureCount = options.Algorithm.SignatureSize;
            }
            else
            {
                signatureCount = 0;
                for (int i = token.Length - 1; i >= options.PreComputedHeader.Length + 1; i--)
                {
                    signatureCount++;
                    if (token[i] == '.')
                    {
                        signatureOffset = token.Length - signatureCount - 1;
                        break;
                    }
                }
            }
            
            // Ensure that signature positions have been found
            if (signatureOffset == -1 || signatureCount == -1)
            {
                parts = default;
                return false;
            }
            
            // Token should be in alright format, using obtained data generate payload offsets
            int payloadOffset = options.PreComputedHeader.Length + 1;
            int payloadCount = token.Length - headerCount - signatureCount - 2; // -2, because of separator dots
            
            // Wrap to struct & return true
            parts = new TokenParts(headerOffset, headerCount, payloadOffset, payloadCount, signatureOffset, signatureCount);
            return true;
        }

        // Checks if computed signature matches signature token part
        private static bool ValidateSignature(ArraySegment<char> computed, TokenPart signaturePart)
        {
            if (computed.Array == null) return false;
            if (computed.Count != signaturePart.Count) return false;

            int computedOffset = computed.Offset;
            for (int i = 0; i < computed.Count; i++)
            {
                if (computed.Array[computedOffset + i] != signaturePart.Target[signaturePart.Offset + i])
                    return false;
            }

            return true;
        }
        
        // Used to validate built-in date-related claims
        private static JwtValidationResult ValidateClaims<T>(T payload)
        {
            // Validate date-related claims
            DateTimeOffset now = DateTimeOffset.UtcNow;

            if (payload is IIssuedAtClaim iat)
            {
                // Check if token isn't issued in future
                if (iat.Iat > now) return JwtValidationResult.IssuedInFuture; 
            }

            if (payload is INotBeforeClaim nbf)
            {
                // Check if not trying to use it too early
                if (nbf.Nbf > now) return JwtValidationResult.UsedTooEarly;
            }

            if (payload is IExpirationClaim exp)
            {
                // Check if it's not expired
                if (exp.Exp < now) return JwtValidationResult.Expired;
            }

            return JwtValidationResult.Success;
        }

        // Helper struct to wrap JWT token parts
        private readonly struct TokenParts
        {
            public readonly int HeaderOffset;
            public readonly int HeaderCount;
            public readonly int PayloadOffset;
            public readonly int PayloadCount;
            public readonly int SignatureOffset;
            public readonly int SignatureCount;


            public TokenParts(int headerOffset, int headerCount, int payloadOffset, int payloadCount, int signatureOffset, int signatureCount)
            {
                HeaderOffset = headerOffset;
                HeaderCount = headerCount;
                PayloadOffset = payloadOffset;
                PayloadCount = payloadCount;
                SignatureOffset = signatureOffset;
                SignatureCount = signatureCount;
            }
        }
    }
}