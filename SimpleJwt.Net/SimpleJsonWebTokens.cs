using System;
using System.Text;
using System.Text.Json;
using Exyll;
using LambdaTheDev.SimpleJwt.Net.Algorithms;
using LambdaTheDev.SimpleJwt.Net.Claims;
using LambdaTheDev.SimpleJwt.Net.StringUtils;

namespace LambdaTheDev.SimpleJwt.Net
{
    // Most important class in this library. Allows to generate & validate tokens
    public sealed class SimpleJsonWebTokens
    {
        private readonly Base64Encoder _base64 = Base64Encoder.UrlEncoding.CopyEncoder(); // Base64 encoder
        private readonly EncodingWrapper _utf8 = new EncodingWrapper(Encoding.UTF8); // todo: Make sure that I don't have to make instance of encoding
        private readonly IJwtAlgorithm _algorithm; // Algorithm used by this JWT generator
        private readonly string _encodedHeader; // JWT header, created once
        
        public IJwtAlgorithm Algorithm => _algorithm; // Algorithm is exposed publicly, so some secret keys etc can be changed
        internal string EncodedHeader => _encodedHeader; // Used for testing proposes
        
        
        public SimpleJsonWebTokens(IJwtAlgorithm algorithm)
        {
            _algorithm = algorithm;
            
            // Serialize & cache header once
            JwtHeader header = new JwtHeader(_algorithm.Name);
            string headerJson = JsonSerializer.Serialize(header);
            StringSegment headerJsonSegment = new StringSegment(headerJson, 0, headerJson.Length);
            _utf8.Append(headerJsonSegment);
            _encodedHeader = _base64.ToBase(_utf8.ToReusableBuffer());
            _utf8.Clear();
        }
        
        // Generates a token where T type is a payload.
        public string Generate<T>(T payload)
        {
            // Generate payload
            string jsonPayload = JsonSerializer.Serialize(payload);
            _utf8.Clear();
            _utf8.Append(new StringSegment(jsonPayload));
            string base64Payload = _base64.ToBase(_utf8.ToReusableBuffer());
         
            // Generate signature
            _utf8.Clear();
            _utf8.Append(new StringSegment(_encodedHeader), '.');
            _utf8.Append(new StringSegment(base64Payload), '.');
            
            byte[] computedHash = _algorithm.Hash(_utf8.ToReusableBuffer());
            string signature = _base64.ToBase(new ArraySegment<byte>(computedHash));

            // Return generated token
            return string.Join(".", _encodedHeader, base64Payload, signature);
        }

        // Validates JWT token. Throws InvalidTokenException, if it's invalid
        public T Validate<T>(string token)
        {
            try
            {
                StringSegment header = default, payload = default, signature = default;
                
                // First obtain all parts of token & ensure that it's valid
                StringIterator iterator = new StringIterator(token, '.');
                int iterations = 0;
                while (iterator.MoveNext())
                {
                    iterations++;
                    
                    if (iterations == 1)
                        header = iterator.Current();
                    
                    else if (iterations == 2)
                        payload = iterator.Current();
                    
                    else if (iterations == 3)
                        signature = iterator.Current();
                    
                    // If more than 3 iterations are made, then token is too long
                    else
                        throw new InvalidTokenException(JwtFailureCause.InvalidFormat);
                }
                
                // If 3 iterations weren't made, then token is too short
                if(iterations != 3)
                    throw new InvalidTokenException(JwtFailureCause.InvalidFormat);

                // Validate segments
                if(!header.Equals(_encodedHeader))
                    throw new InvalidTokenException(JwtFailureCause.InvalidHeader);
                
                // Validate signature
                _utf8.Clear();
                _utf8.Append(header, '.');
                _utf8.Append(payload, '.');

                byte[] computedHash = _algorithm.Hash(_utf8.ToReusableBuffer());
                string signatureBase64 = _base64.ToBase(new ArraySegment<byte>(computedHash));
                
                if(!signature.Equals(signatureBase64))
                    throw new InvalidTokenException(JwtFailureCause.InvalidSignature);
                
                // Now... only decode payload & validate basic claims
                ArraySegment<byte> base64Decoded = _base64.FromBase(payload);
                _utf8.Clear();
                string rawPayload = _utf8.GetString(base64Decoded);

                T deserializedPayload = JsonSerializer.Deserialize<T>(rawPayload);
                EnsureClaimsAreValid(deserializedPayload);
                
                // And return payload for further validation
                return deserializedPayload;
            }
            catch (InvalidTokenException) { throw; }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new InvalidTokenException(JwtFailureCause.Unknown);
            }
        }

        // Gets values from Claims interfaces & validates them
        private void EnsureClaimsAreValid<T>(T payload)
        {
            if (payload is IExpirationClaim exp)
            {
                if(exp.Exp <= DateTime.UtcNow)
                    throw new InvalidTokenException(JwtFailureCause.Expired);
            }

            if (payload is IIssuedAtClaim iat)
            {
                if(iat.Iat > DateTime.UtcNow)
                    throw new InvalidTokenException(JwtFailureCause.IssuedInFuture);
            }

            if (payload is INotBeforeClaim nbf)
            {
                if(nbf.Nbf > DateTime.UtcNow)
                    throw new InvalidTokenException(JwtFailureCause.UsedTooEarly);
            }
        }
    }
}