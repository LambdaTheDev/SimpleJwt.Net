using System;
using System.Text;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class DebugTests
    {
        [Test]
        public void Utf8CharTest()
        {
            Console.WriteLine(Encoding.UTF8.BodyName);
        }
    }
}