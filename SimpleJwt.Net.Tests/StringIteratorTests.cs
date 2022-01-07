using System;
using System.Text;
using LambdaTheDev.SimpleJwt.Net.StringUtils;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class StringIteratorTests
    {
        private readonly StringBuilder _builder = new StringBuilder();
        
        private string[] _testStrings = new[]
        {
            "aaa.bbb.ccc",
            "aaaaaaaa",
            "aaa.bbb",
            "aaaaaaa.",
            "aaaaa.bbbbb.",
            ".aaaaa",
            ".aaaaa.bbbbb",
            ".",
            "..",
            "..."
        };
        
        [Test]
        public void IteratorTest()
        {
            bool success = true;
            int passed = 0;

            for (int i = 0; i < _testStrings.Length; i++)
            {
                bool stringSuccessful = true;
                StringIterator iterator = new StringIterator(_testStrings[i], '.');
                string[] split = _testStrings[i].Split('.');

                int iterations = 0;
                while (iterator.MoveNext())
                {
                    string entry = BuildString(iterator.Current());
                    if (split[iterations] != entry)
                    {
                        stringSuccessful = false;
                        Console.WriteLine("Strings do not match!\nOriginal: " + _testStrings[i] + "\n" +
                                          "Current iteration: " + iterations + "\nExpected value: " + split[iterations] + "\n" +
                                          "Actual value: " + entry + "\n\n");
                    }

                    iterations++;
                }

                if (stringSuccessful)
                {
                    if (split.Length == iterations)
                    {
                        Console.WriteLine("String " + _testStrings[i] + " passed test!\n\n");
                        passed++;
                    }
                    else
                    {
                        Console.WriteLine("String " + _testStrings[i] + " has invalid number of iterations (expected: " + split.Length + ", got: " + iterations + ")!\n\n");
                    }
                }

                if (!stringSuccessful && success)
                    success = false;
            }
            
            Console.WriteLine(passed + " out of " + _testStrings.Length + " strings passed!");
            Assert.True(success);
        }

        private string BuildString(StringIteratorEntry entry)
        {
            if (entry.IsNull)
                return null;

            if (entry.IsEmpty)
                return string.Empty;

            _builder.Clear();
            for (int i = entry.Offset; i < entry.Offset + entry.Count; i++)
            {
                char x = entry.OriginalString[i];
                _builder.Append(x);
            }

            return _builder.ToString();
        }
    }
}