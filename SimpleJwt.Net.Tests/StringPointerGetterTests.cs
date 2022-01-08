using System;
using LambdaTheDev.SimpleJwt.Net.StringUtils;
using LambdaTheDev.SimpleJwt.Net.Utils;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class StringPointerGetterTests
    {
        [Test]
        public void BasicTest()
        {
            if (!SimpleJwtInfo.IsExperimental)
                Assert.Pass();
            
            string x = "dcba";

            if (StringPointerGetter.TryGetFor(x, out char value))
            {
                Console.WriteLine("String: " + x + "\nFirst char: " + value);
                Assert.True(value == x[0]);
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}