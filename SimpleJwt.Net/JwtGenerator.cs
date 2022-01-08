using System.Text;
using System.Text.Json;
using Exyll;
using LambdaTheDev.SimpleJwt.Net.Algorithms;
using LambdaTheDev.SimpleJwt.Net.StringUtils;

namespace LambdaTheDev.SimpleJwt.Net
{
    // First most important class for common users, it generates JWT token 
    public sealed class JwtGenerator
    {
        private readonly Base64Encoder _base64 = Base64Encoder.UrlEncoding.CopyEncoder(); // Base64 encoder
        private readonly EncodingWrapper _utf8 = new EncodingWrapper(Encoding.UTF8); // todo: Make sure that I don't have to make instance of encoding
        private readonly IJwtAlgorithm _algorithm; // Algorithm used by this JWT generator
        private readonly string _encodedHeader; // JWT header, created once
        
        public IJwtAlgorithm Algorithm => _algorithm; // Algorithm is exposed publicly, so some secret keys etc can be changed

        
        public JwtGenerator(IJwtAlgorithm algorithm)
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
            _utf8.Append(new StringSegment(_encodedHeader), '.');
            _utf8.Append(new StringSegment(base64Payload), '.');
            
            byte[] computedHash = _algorithm.Hash(_utf8.ToReusableBuffer());
            string signature = _base64.ToBase(computedHash);

            // Return generated token
            return string.Join('.', _encodedHeader, base64Payload, signature);
        }
    }
}