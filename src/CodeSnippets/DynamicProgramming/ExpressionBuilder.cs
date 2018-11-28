using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CodeSnippets.DynamicProgramming
{
    public static class ExpressionBuilder
    {
        public static Expression DictionaryContainsKeyExpression(Expression dictionaryExpr, Expression keyExpr)
        {
            return Expression.Call(dictionaryExpr, "ContainsKey", null, keyExpr);
        }

        public static Expression GetValueFromDictionaryAsString(Expression dictionaryExpr, Expression keyExpr)
        {
            var resultExpr = Expression.Variable(typeof(string));
            var assignExpression = Expression.Assign(resultExpr, Expression.Property(dictionaryExpr, "Item", keyExpr));
            return Expression.Block(new[] { resultExpr },
                assignExpression,
                resultExpr);
        }

        public static Expression GetValueFromDictionaryAsInt(Expression dictionaryExpr, Expression keyExpr)
        {
            // create variables
            var resultInt = Expression.Variable(typeof(int), "resultInt");
            var dictionaryLookupResult = Expression.Variable(typeof(string), "dictionaryLookupResult");

            // method call to int.Parse(string)
            var parseMethod = typeof(int).GetMethod("Parse", new[] { typeof(string) });

            // assign result from dictionary lookup result
            var assignDictionaryLookup = Expression.Assign(dictionaryLookupResult, Expression.Property(dictionaryExpr, "Item", keyExpr));

            // parse result from dictionary
            var parseExpr = Expression.Call(parseMethod, dictionaryLookupResult);

            // assign result of parse to our return object
            var assignToEnumResultExpr = Expression.Assign(resultInt, parseExpr);

            return Expression.Block(new[] { resultInt },
                assignDictionaryLookup,
                assignToEnumResultExpr,
                resultInt);
        }

        public static Expression GetValueFromDictionaryAsLong(Expression dictionaryExpr, Expression keyExpr)
        {
            // create variables
            var resultLong = Expression.Variable(typeof(long), "resultLong");
            var dictionaryLookupResult = Expression.Variable(typeof(string), "dictionaryLookupResult");

            // method call to long.Parse(string)
            var parseMethod = typeof(long).GetMethod("Parse", new[] { typeof(string) });

            // assign result from dictionary lookup result
            var assignDictionaryLookup = Expression.Assign(dictionaryLookupResult, Expression.Property(dictionaryExpr, "Item", keyExpr));

            // parse result from dictionary
            var parseExpr = Expression.Call(parseMethod, dictionaryLookupResult);

            // assign result of parse to our return object
            var assignToEnumResultExpr = Expression.Assign(resultLong, parseExpr);

            return Expression.Block(new[] { resultLong },
                assignDictionaryLookup,
                assignToEnumResultExpr,
                resultLong);
        }

        public static Expression GetValueFromDictionaryAsDecimal(Expression dictionaryExpr, Expression keyExpr)
        {
            // create variables
            var resultDecimal = Expression.Variable(typeof(decimal), "resultDecimal");
            var dictionaryLookupResult = Expression.Variable(typeof(string), "dictionaryLookupResult");

            // method call to decimal.Parse(string)
            var parseMethod = typeof(decimal).GetMethod("Parse", new[] { typeof(string) });

            // assign result from dictionary lookup result
            var assignDictionaryLookup = Expression.Assign(dictionaryLookupResult, Expression.Property(dictionaryExpr, "Item", keyExpr));

            // parse result from dictionary
            var parseExpr = Expression.Call(parseMethod, dictionaryLookupResult);

            // assign result of parse to our return object
            var assignToEnumResultExpr = Expression.Assign(resultDecimal, parseExpr);

            return Expression.Block(new[] { resultDecimal },
                assignDictionaryLookup,
                assignToEnumResultExpr,
                resultDecimal);
        }

        public static Expression GetValueFromDictionaryAsTimespan(Expression dictionaryExpr, Expression keyExpr)
        {
            // create variables
            var resultTimeSpan = Expression.Variable(typeof(TimeSpan), "resultTimeSpan");
            var ticksVar = Expression.Variable(typeof(long), "ticksVar");
            var dictionaryLookupResult = Expression.Variable(typeof(string), "dictionaryLookupResult");

            // method call to long.Parse(string)
            var parseMethod = typeof(long).GetMethod("Parse", new[] { typeof(string) });
            
            // assign result from dictionary lookup result
            var assignDictionaryLookup = Expression.Assign(dictionaryLookupResult, Expression.Property(dictionaryExpr, "Item", keyExpr));

            // parse result from dictionary
            var parseExpr = Expression.Call(parseMethod, dictionaryLookupResult);

            // assign result of parse to our return object
            var assignParseResultExpr = Expression.Assign(ticksVar, parseExpr);

            // method cal to 
            var timeSpanCtor = Expression.New(typeof(TimeSpan).GetConstructor(new[] { typeof(long) }), ticksVar);
            var assignToResultExpr = Expression.Assign(resultTimeSpan, timeSpanCtor);

            return Expression.Block(new[] { resultTimeSpan },
                assignDictionaryLookup,
                assignParseResultExpr,
                assignToResultExpr,
                resultTimeSpan);
        }

        public static Expression GetValueFromDictionaryAsEnum(Expression dictionaryExpr, Expression keyExpr, Type enumType)
        {
            if (!enumType.IsEnum)
                throw new Exception("Expected type enum");

            // create variables
            var resultEnum = Expression.Variable(enumType, "resultEnum");
            var dictionaryLookupResult = Expression.Variable(typeof(string), "dictionaryLookupResult");
            var resultTypeConst = Expression.Constant(enumType);
            var @true = Expression.Constant(true);
            
            // method call to Enum.Parse(Type, string, bool)
            var parseMethod = typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string), typeof(bool) } );

            // assign result from dictionary lookup result
            var assignDictionaryLookup = Expression.Assign(dictionaryLookupResult, Expression.Property(dictionaryExpr, "Item", keyExpr));

            // parse result from dictionary
            var parseExpr = Expression.Call(parseMethod, resultTypeConst, dictionaryLookupResult, @true);
            var castToEnumType = Expression.Convert(parseExpr, enumType);

            // assign result of parse to our return object
            var assignToEnumResultExpr = Expression.Assign(resultEnum, castToEnumType);

            return Expression.Block(new[] { resultEnum },
                assignDictionaryLookup,
                assignToEnumResultExpr,
                resultEnum);
        }
    }
}
