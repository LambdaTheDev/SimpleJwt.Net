using System;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class DebugTests
    {
        [Test]
        public void Utf8CharTest()
        {
            char ch = 'Ǿ';
            
            Console.WriteLine(ch);
        }
    }
}