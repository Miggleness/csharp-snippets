using CodeSnippets.Expressions;
using Shouldly;
using System;
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

        /// <summary>
        /// Creates an object from a given specs in dictionary
        /// </summary>
        [Fact]
        [Trait("Category", "BuildLambdaCreateObjectFromDictionarySpecs")]
        public void Should_create_object_from_dictionary_specs()
        {
            var input = new Dictionary<string, string>()
            {
                {"Id", "1" },
                {"LongId", "12147483647" },
                {"Name", "Martin" },
                {"Type", "Val2" },
                {"Money", "1.0003" },
                {"OffsetBy", "864000000000" },
                {"Birthday", "2008-11-01T19:35:00.0000000Z" },
                {"DDay", "2008-11-01T19:35:00.0000000-07:00" },
                {"Approximation", "1.1234567" },
                {"LargerApproximation", "1.123456789" },
                {"EmptyChar", "" },
                {"OneChar", "a" },
                {"BoolText", "true" },
                {"BoolNum", "1" }
            };


            var func = ExpressionSnippets.BuildLambdaCreateObjectFromDictionarySpecs(input, typeof(MyInstantiatedClass));
            var result = func() as MyInstantiatedClass;
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.LongId.ShouldBe(12147483647);
            result.Name.ShouldBe("Martin");
            result.Type.ShouldBe(MyInstantiatedClassEnum.Val2);
            result.Money.ShouldBe(1.0003m);
            result.OffsetBy.ShouldBe(TimeSpan.FromTicks(864000000000));
            result.Birthday.ShouldBe(DateTime.Parse("2008-11-01T19:35:00.0000000Z"));
            result.DDay.ShouldBe(DateTimeOffset.Parse("2008-11-01T19:35:00.0000000-07:00"));
            result.Approximation.ShouldBe(1.1234567f);
            result.LargerApproximation.ShouldBe(1.123456789);
            result.EmptyChar.ShouldBe(default(char));
            result.OneChar.ShouldBe('a');
            result.BoolText.ShouldBeTrue();
            result.BoolNum.ShouldBeTrue();
        }

        [Fact]
        [Trait("Category", "BuildLambdaCreateObjectFromDictionarySpecs")]
        public void Should_fail_for_unsupported_type()
        {
            var input = new Dictionary<string, string>()
            {
                {"Obj", string.Empty },
            };

            var e = Should.Throw<Exception>(() => ExpressionSnippets.BuildLambdaCreateObjectFromDictionarySpecs(input, typeof(MyInstantiatedClassWithObjectProperty)));
            e.Message.ShouldBe("Cannot assign value to property. Type unsupported.");
        }
    }
}
