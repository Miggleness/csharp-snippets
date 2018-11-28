using Jil;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CodeSnippets.DynamicProgramming
{
    public static partial class ExpressionBuilder
    {
        public static Expression DictionaryContainsKeyExpression(Expression dictionaryExpr, Expression keyExpr)
        {
            return Expression.Call(dictionaryExpr, "ContainsKey", null, keyExpr);
        }

        public static Expression GetValueFromDictionaryAsString(Expression dictionaryExpr, Expression keyExpr)
        {
            var resultVar = Expression.Variable(typeof(string));
            var assignExpression = Expression.Assign(resultVar, Expression.Property(dictionaryExpr, "Item", keyExpr));
            return Expression.Block(new[] { resultVar },
                assignExpression,
                resultVar);
        }

        public static Expression GetValueFromDictionaryAs<T>(Expression dictionaryExpr, Expression keyExpr)
        {
            bool IsTypeSupported(Type t)
            {
                return t == typeof(int)
                    || t == typeof(long)
                    || t == typeof(decimal)
                    || t == typeof(float)
                    || t == typeof(double)
                    || t == typeof(bool);
            }

            var type = typeof(T);

            if (!IsTypeSupported(type))
                throw new Exception($"Cannot use 'GetValueFromDictionaryAs<T>' for type {type.Name}");

            // create variables
            var resultVar = Expression.Variable(type, "parseResult");
            var dictionaryLookupResult = Expression.Variable(typeof(string), "dictionaryLookupResult");

            // assign result from dictionary lookup result
            var assignDictionaryLookupExpr = Expression.Assign(dictionaryLookupResult, Expression.Property(dictionaryExpr, "Item", keyExpr));

            // assign result of double.Parse against our value
            var assignParseResultExpr = Expression.Assign(
                left: resultVar,
                right: Expression.Call(
                    method: type.GetMethod("Parse", new[] { typeof(string) }),
                    arguments: new[] { dictionaryLookupResult }));

            return Expression.Block(new[] { resultVar },
                assignDictionaryLookupExpr,
                assignParseResultExpr,
                resultVar);
        }

        public static Expression GetValueFromDictionaryAsDateTime(Expression dictionaryExpr, Expression keyExpr)
        {
            // create variables
            var resultVar = Expression.Variable(typeof(DateTime), "resultDateTime");
            var dictionaryLookupResult = Expression.Variable(typeof(string), "dictionaryLookupResult");

            // assign result from dictionary lookup result
            var assignDictionaryLookupExpr = Expression.Assign(dictionaryLookupResult, Expression.Property(dictionaryExpr, "Item", keyExpr));
            // assign result of DateTime.Parse(string, IFormatProvider,  against our value
            var assignParseResultExpr = Expression.Assign(
                left: resultVar,
                right: Expression.Call(
                    method: typeof(DateTime).GetMethod("Parse", new[] { typeof(string), typeof(IFormatProvider), typeof(DateTimeStyles) }),
                    arguments: new Expression[] { dictionaryLookupResult, Expression.Constant(DateTimeFormatInfo.InvariantInfo), Expression.Constant(DateTimeStyles.None) }));

            return Expression.Block(new[] { resultVar },
                assignDictionaryLookupExpr,
                assignParseResultExpr,
                resultVar);
        }

        public static Expression GetValueFromDictionaryAsDateTimeOffset(Expression dictionaryExpr, Expression keyExpr)
        {
            // create variables
            var resultVar = Expression.Variable(typeof(DateTimeOffset), "resultDateTimeOffset");
            var dictionaryLookupResult = Expression.Variable(typeof(string), "dictionaryLookupResult");

            // assign result from dictionary lookup result
            var assignDictionaryLookupExpr = Expression.Assign(dictionaryLookupResult, Expression.Property(dictionaryExpr, "Item", keyExpr));
            // assign result of DateTimeOffset.Parse(string, IFormatProvider,  against our value
            var assignParseResultExpr = Expression.Assign(
                left: resultVar,
                right: Expression.Call(
                    method: typeof(DateTimeOffset).GetMethod("Parse", new[] { typeof(string), typeof(IFormatProvider), typeof(DateTimeStyles) }),
                    arguments: new Expression[] { dictionaryLookupResult, Expression.Constant(DateTimeFormatInfo.InvariantInfo), Expression.Constant(DateTimeStyles.None) }));

            return Expression.Block(new[] { resultVar },
                assignDictionaryLookupExpr,
                assignParseResultExpr,
                resultVar);
        }

        public static Expression GetValueFromDictionaryAsChar(Expression dictionaryExpr, Expression keyExpr)
        {
            // create variables
            var resultVar = Expression.Variable(typeof(char), "resultChar");
            var dictionaryLookupResult = Expression.Variable(typeof(string), "dictionaryLookupResult");
            var zero = Expression.Constant(0);
            
            // assign result from dictionary lookup result
            var assignDictionaryLookupExpr = Expression.Assign(dictionaryLookupResult, Expression.Property(dictionaryExpr, "Item", keyExpr));

            // check length and assign value
            var ifElseThenExpr = Expression.IfThenElse(
                Expression.GreaterThan(
                    Expression.Property(dictionaryLookupResult, "Length"),
                    zero),
                Expression.Assign(resultVar, Expression.Property(dictionaryLookupResult, "Chars", zero)),
                Expression.Assign(resultVar, Expression.Constant(char.MinValue))
            );

            return Expression.Block(new[] { resultVar },
                assignDictionaryLookupExpr,
                ifElseThenExpr,
                resultVar);
        }

        public static Expression GetValueFromDictionaryAsTimespan(Expression dictionaryExpr, Expression keyExpr)
        {
            // create variables
            var resultVar = Expression.Variable(typeof(TimeSpan), "resultTimeSpan");
            var ticksVar = Expression.Variable(typeof(long), "ticksVar");
            var dictionaryLookupResult = Expression.Variable(typeof(string), "dictionaryLookupResult");

            // assign result from dictionary lookup result
            var assignDictionaryLookupExpr = Expression.Assign(dictionaryLookupResult, Expression.Property(dictionaryExpr, "Item", keyExpr));

            // assign result of parse to our return object
            var assignParseResultExpr = Expression.Assign(
                left: ticksVar, 
                right: Expression.Call(
                    method: typeof(long).GetMethod("Parse", new[] { typeof(string) }),
                    arguments: dictionaryLookupResult));

            // assign
            var assignToResultExpr = Expression.Assign(
                left: resultVar,
                right: Expression.New(typeof(TimeSpan).GetConstructor(new[] { typeof(long) }), ticksVar)
                );

            return Expression.Block(new[] { resultVar },
                assignDictionaryLookupExpr,
                assignParseResultExpr,
                assignToResultExpr,
                resultVar);
        }

        public static Expression GetValueFromDictionaryAsEnum(Expression dictionaryExpr, Expression keyExpr, Type enumType)
        {
            if (!enumType.IsEnum)
                throw new Exception("Expected type enum");

            // create variables
            var resultVar = Expression.Variable(enumType, "resultEnum");
            var dictionaryLookupResult = Expression.Variable(typeof(string), "dictionaryLookupResult");
            var resultTypeConst = Expression.Constant(enumType);
            var @true = Expression.Constant(true);
            
            // assign result from dictionary lookup result
            var assignDictionaryLookupExpr = Expression.Assign(dictionaryLookupResult, Expression.Property(dictionaryExpr, "Item", keyExpr));

            // assign result of parse to our return object
            var assignParseResultExpr = Expression.Assign(
                left: resultVar, 
                right: Expression.Convert(
                    expression: Expression.Call(
                            method: typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string), typeof(bool) }),
                            arguments: new Expression[] { resultTypeConst, dictionaryLookupResult, @true }),
                    type: enumType));

            return Expression.Block(new[] { resultVar },
                assignDictionaryLookupExpr,
                assignParseResultExpr,
                resultVar);
        }

        public static Expression GetValueFromDictionaryAsList(Expression dictionaryExpr, Expression keyExpr, Type listType)
        {
            bool IsGenericTypeSupported(Type t)
            {
                return t == typeof(string)
                    || t == typeof(int);
            }

            var genericType = listType.GetGenericArguments()[0];
            if (!IsGenericTypeSupported(genericType))
                throw new Exception($"Cannot use 'GetValueFromDictionaryAsList<T>' for type {genericType.Name}");

            // create variables
            var resultVar = Expression.Variable(listType, "resultList");
            var dictionaryLookupResult = Expression.Variable(typeof(string), "dictionaryLookupResult");

            // assign result from dictionary lookup result
            var assignDictionaryLookupExpr = Expression.Assign(dictionaryLookupResult, Expression.Property(dictionaryExpr, "Item", keyExpr));

            // assign result of double.Parse against our value
            var assignParseResultExpr = Expression.Assign(
                left: resultVar,
                right: Expression.Call(
                    typeof(JSON), "Deserialize",
                    typeArguments: new Type[] { listType },
                    arguments: new Expression[] { dictionaryLookupResult, Expression.Constant(null, typeof(Options)) }));

            return Expression.Block(new[] { resultVar },
                assignDictionaryLookupExpr,
                assignParseResultExpr,
                resultVar);

            //var genericType = prop.PropertyType.GenericTypeArguments.FirstOrDefault();
            //var dataConst = Expression.Constant(data, typeof(string));
            //var propTypeConst = Expression.Constant(prop.PropertyType, typeof(Type));
            //var optionsConst = Expression.Constant(null, typeof(Options));

            //var callExpr = 
            //    
            //    

            //return Expression.Bind(prop, callExpr);
        }
    }
}
