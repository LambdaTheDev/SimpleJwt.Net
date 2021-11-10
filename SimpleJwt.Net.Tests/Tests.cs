using System;
using NUnit.Framework;
using SimpleJwt.Net.Algorithms;
using SimpleJwt.Net.Exception;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class Tests
    {
    }

    internal struct SimpleJwt : IJwtPayload
    {
        public string Iss { get; set; }
        public string Sub { get; set; }
        public DateTime Iat { get; set; }
        public DateTime Exp { get; set; }
    }
}