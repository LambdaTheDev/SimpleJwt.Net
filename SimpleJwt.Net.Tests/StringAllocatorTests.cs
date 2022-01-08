using LambdaTheDev.SimpleJwt.Net.StringUtils;
using LambdaTheDev.SimpleJwt.Net.Utils;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class StringAllocatorTests
    {
        [Test]
        public void BasicTest()
        {
            if (!SimpleJwtInfo.IsExperimental)
                Assert.Pass();
            
            string value = StringAllocator.Allocate(20);
            Assert.True(value.Length == 20);
        }
    }
}