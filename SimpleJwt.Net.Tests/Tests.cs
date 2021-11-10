using System;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class Tests
    {
    }

    internal struct SimpleJwtBasic : IJwtBasicPayload
    {
        public string Iss { get; set; }
        public string Sub { get; set; }
        public DateTime Iat { get; set; }
        public DateTime Exp { get; set; }
    }
}