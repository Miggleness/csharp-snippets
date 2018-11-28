using CodeSnippets.DynamicProgramming;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace CodeSnippets.Tests
{
    public class ExpressionBuilderTests
    {
        [Fact]
        public void DictionaryContainsKeyExpression_should_assert()
        {
            var dictionaryParameter = Expression.Parameter(typeof(Dictionary<string, string>), "dictionary");

            var key = Expression.Parameter(typeof(string), "key");

            var containsKeyCheck = ExpressionBuilder.DictionaryContainsKey(dictionaryParameter, key);
            var lambdaExpr = Expression.Lambda<Func<Dictionary<string, string>, string, bool>>(containsKeyCheck, dictionaryParameter, key);
            var lambda = lambdaExpr.Compile();

            var dictionary = new Dictionary<string, string>
            {
                {"Key1", "Value1" }
            };

            lambda(dictionary, "Key1").ShouldBeTrue();
            lambda(dictionary, "NoKey").ShouldBeFalse();
        }

        [Fact]
        public void GetValueFromDictionary_should_assert()
        {
            var dictionaryParameter = Expression.Parameter(typeof(Dictionary<string, string>), "dictionary");

            var key = Expression.Constant("Key1");
            var valueResult = Expression.Parameter(typeof(string));
            var containsKeyCheck = ExpressionBuilder.GetValueFromDictionaryAsString(dictionaryParameter, key, typeof(string));
            var lambdaExpr = Expression.Lambda<Func<Dictionary<string, string>, string>>(containsKeyCheck, dictionaryParameter);
            var lambda = lambdaExpr.Compile();

            var dictionary = new Dictionary<string, string>
            {
                {"Key1", "Value1" }
            };

            lambda(dictionary).ShouldBe("Value1"); 
        }
    }
}
