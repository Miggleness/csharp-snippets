using CodeSnippets.Roslyn;
using Shouldly;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace CodeSnippets.Tests
{
    public class RoslynTests
    {
        public static ITestOutputHelper _out;

        public RoslynTests(ITestOutputHelper @out)
        {
            _out = @out;

        }

        [Fact]
        public void Should_compile_string_to_lambda()
        {
            var input = new Dictionary<string, string>()
            {
                {"Id", "1" },
                {"Type", "MyInstantiatedClassEnum.Val1" }
            };

            var func = RoslynSnippets.BuildLambdaCreateObjectFromDictionarySpecs(input, typeof(MyInstantiatedClass));
            var result = func();
            result.ShouldBeOfType<MyInstantiatedClass>();

        }
    }
}
