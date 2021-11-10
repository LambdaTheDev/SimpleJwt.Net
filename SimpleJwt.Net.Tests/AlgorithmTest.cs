using System;
using NUnit.Framework;
using SimpleJwt.Net.Algorithms;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class AlgorithmTest
    {
        private readonly Hs256 _algo = new Hs256("abc123");

        [Test]
        public void HashTest()
        {
            Console.WriteLine(_algo.Hash("testString123"));
        }
    }
}