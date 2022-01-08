using System;
using System.Text;
using LambdaTheDev.SimpleJwt.Net.StringUtils;
using LambdaTheDev.SimpleJwt.Net.Utils;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class EncodingWrapperTests
    {
        [Test]
        public void SimpleTest()
        {
            EncodingWrapper wrapper = new EncodingWrapper(Encoding.UTF8);
            UTF8Encoding encoding = new UTF8Encoding();

            string str = "testString";
            wrapper.Append(new StringSegment(str, 0, str.Length));

            byte[] bytes = encoding.GetBytes(str);
            byte[] alloc = wrapper.ToAllocatedArray();

            if (bytes.Length == alloc.Length)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    if(bytes[i] != alloc[i])
                        Assert.Fail();
                }
                
                Assert.Pass();
            }
            
            Assert.Fail();
        }

        [Test]
        public void PartialStringTest()
        {
            EncodingWrapper wrapper = new EncodingWrapper(Encoding.UTF8);
            UTF8Encoding encoding = new UTF8Encoding();

            string testString = "testSuperString";
            string subStrAlloc = testString.Substring(4, testString.Length - 4);
            StringSegment segment = new StringSegment(testString, 4, testString.Length - 4);
            
            if (subStrAlloc != segment.ToString())
            {
                Console.WriteLine("Substring: " + subStrAlloc + "\nSegment: " + segment.ToString());
                Assert.Fail();
            }
            
            wrapper.Append(segment);
            byte[] wrapperResult = wrapper.ToAllocatedArray();
            byte[] encodingResult = encoding.GetBytes(subStrAlloc);

            if (wrapperResult.Length == encodingResult.Length)
            {
                for (int i = 0; i < wrapperResult.Length; i++)
                {
                    if(wrapperResult[i] != encodingResult[i])
                        Assert.Fail();
                }
                
                Assert.Pass();
            }

            Assert.Fail("Wrapper length: " + wrapperResult.Length + ";\nEncoding length: " + encodingResult.Length);
        }

        [Test]
        public void AppendedStringTest()
        {
            EncodingWrapper wrapper = new EncodingWrapper(Encoding.UTF8);
            UTF8Encoding decoder = new UTF8Encoding();
            
            string partA = "wrapperTest";
            string partB = "secondPart";
            char separator = '.';
            
            wrapper.Append(new StringSegment(partA, 0, partA.Length), separator);
            byte[] bytesA = wrapper.ToAllocatedArray();
            
            wrapper.Append(new StringSegment(partB, 0, partB.Length), separator);
            byte[] bytesB = wrapper.ToAllocatedArray();

            string appendedResultA = decoder.GetString(bytesA);
            string appendedResultB = decoder.GetString(bytesB);

            // If separator got appended on first write
            if(appendedResultA.StartsWith('.'))
                Assert.Fail();

            string compString = string.Join(separator, partA, partB); 
            Assert.True(compString == appendedResultB);
        }
    }
}