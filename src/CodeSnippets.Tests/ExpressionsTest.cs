using CodeSnippets.Expressions;
using Shouldly;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace CodeSnippets.Tests
{
    public class ExpressionsTest
    {
        public static ITestOutputHelper _out;

        public ExpressionsTest(ITestOutputHelper @out)
        {
            _out = @out;

        }

        [Fact]
        public void Should_compile_string_to_lambda()
        {
            var input = new Dictionary<string, string>()
            {
                {"Id", "1" },
                {"Type", "Val2" }
            };

            var func = ExpressionSnippets.BuildLambdaCreateObjectFromDictionarySpecs(input, typeof(MyInstantiatedClass));
            var result = func() as MyInstantiatedClass;
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Type.ShouldBe(MyInstantiatedClassEnum.Val2);
        }
    }
}
