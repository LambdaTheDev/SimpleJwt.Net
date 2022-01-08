using System;
using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using LambdaTheDev.SimpleJwt.Net.StringUtils;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests.Performance
{
    [TestFixture]
    [MemoryDiagnoser]
    public class JsonSerializationTests
    {
        private string _jsonString;
        private ExampleData _data;
        private EncodingWrapper _wrapper = new EncodingWrapper(new UTF8Encoding());

        // [Test] to not get stuck for some amount of time
        public void Test()
        {
            var summary = BenchmarkRunner.Run<JsonSerializationTests>(new DebugBuildConfig());
            Console.WriteLine(summary);
        }
        
        [GlobalSetup]
        public void Setup()
        {
            DepthStruct ds = new DepthStruct
            {
                A = "kj09u8y78tj781ye8d7ykjqwy89ykdkq8wd918k",
                B = 893852.49237847239587284f,
                C = 90909890809.489832948238d,
            };
            
            ExampleData data = new ExampleData
            {
                Data1 = 534875639,
                Data2 = 545.4234545345f,
                Data3 = (decimal) 39847.923754782984,
                Data4 = "aoashdcinsacu98uy892y892yfjsy7fy89yy7yuidm4ymry93m9u93u120udiws",
                Data5 = ds,
                Data6 = ds,
                Data7 = "9890789j9ynfs78cft7n8t2jq7ydn7qdy9u0198"
            };

            _data = data;
            _jsonString = JsonSerializer.Serialize(data);
            
            // I want to increase buffers size
            _wrapper.Append(new StringSegment(_jsonString, 0, _jsonString.Length));
            _wrapper.Clear();
        }
        
        
        [Benchmark]
        public void RegularJsonDeserialization()
        {
            ExampleData ed = JsonSerializer.Deserialize<ExampleData>(_jsonString);
        }

        [Benchmark]
        public void WrappedJsonDeserialization()
        {
            StringSegment segment = new StringSegment(_jsonString, 0, _jsonString.Length);
            ExampleData data = segment.DeserializeJson<ExampleData>(_wrapper);
        }

        public struct ExampleData
        {
            public int Data1;
            public float Data2;
            public decimal Data3;
            public string Data4;
            public DepthStruct Data5;
            public DepthStruct Data6;
            public string Data7;
        }

        public struct DepthStruct
        {
            public string A;
            public float B;
            public double C;
        }
    }
}