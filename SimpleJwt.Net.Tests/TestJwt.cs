using System;

namespace SimpleJwt.Net.Tests
{
    public struct TestJwt : IJwtBasicPayload
    {
        public string Iss { get; set; }
        public string Sub { get; set; }
        public DateTime Iat { get; set; }
        public DateTime Exp { get; set; }
        public string Secret { get; set; }
    }
}