using System;
using LambdaTheDev.SimpleJwt.Net.StringUtils;
using LambdaTheDev.SimpleJwt.Net.Utils;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class StringManipulationTests
    {
        [Test]
        public void StringGenerator()
        {
            if (!SimpleJwtInfo.IsExperimental)
                Assert.Pass();
            
            string allocated = StringAllocator.Allocate(20);
            if (StringPointerGetter.TryGetFor(allocated, out char firstChar))
            {
                unsafe
                {
                    fixed (char* ptr = allocated)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            ptr[i] = (char) ('j' + i);
                        }
                    }
                }

                for (int i = 0; i < 20; i++)
                {
                    if (allocated[i] != (char) ('j' + i))
                    {
                        Assert.Fail();
                    }
                }
                
                Console.WriteLine(allocated);
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}