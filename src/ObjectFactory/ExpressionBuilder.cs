using Jil;
using System;
using System.Globalization;
using System.Linq.Expressions;

namespace ObjectFactory
{
    public static partial class ExpressionBuilder
    {
        /// <summary>
        /// Returns an expression making a call to Dictionart<>.ContainsKey method for a given dictionary and key
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <returns></returns>
        public static Expression DictionaryContainsKeyExpression(Expression dictionaryExpr, Expression keyExpr)
        {
            return Expression.Call(dictionaryExpr, "ContainsKey", null, keyExpr);
        }

        /// <summary>
        /// Gets the value of a given dictionary and returns it as string. This assumes that the key exists.
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <returns></returns>
        public static Expression GetValueFromDictionaryAsString(Expression dictionaryExpr, Expression keyExpr)
        {
            var resultVar = Expression.Variable(typeof(string));
            var assignExpression = Expression.Assign(resultVar, Expression.Property(dictionaryExpr, "Item", keyExpr));
            return Expression.Block(new[] { resultVar },
                assignExpression,
                resultVar);
        }

        /// <summary>
        /// Gets the value of a given dictionary and returns it as long. This assumes that the key exists.
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <returns></returns>
        public static Expression GetValueFromDictionaryAsLong(Expression dictionaryExpr, Expression keyExpr)
        {
            return GetValueFromDictionaryAs<long>(dictionaryExpr, keyExpr);
        }

        /// <summary>
        /// Gets the value of a given dictionary and returns it as int. This assumes that the key exists.
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <returns></returns>
        public static Expression GetValueFromDictionaryAsInt(Expression dictionaryExpr, Expression keyExpr)
        {
            return GetValueFromDictionaryAs<int>(dictionaryExpr, keyExpr);
        }

        /// <summary>
        /// Gets the value of a given dictionary and returns it as float. This assumes that the key exists.
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <returns></returns>
        public static Expression GetValueFromDictionaryAsFloat(Expression dictionaryExpr, Expression keyExpr)
        {
            return GetValueFromDictionaryAs<float>(dictionaryExpr, keyExpr);
        }

        /// <summary>
        /// Gets the value of a given dictionary and returns it as decimal. This assumes that the key exists.
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <returns></returns>
        public static Expression GetValueFromDictionaryAsDecimal(Expression dictionaryExpr, Expression keyExpr)
        {
            return GetValueFromDictionaryAs<decimal>(dictionaryExpr, keyExpr);
        }

        /// <summary>
        /// Gets the value of a given dictionary and returns it as double. This assumes that the key exists.
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <returns></returns>
        public static Expression GetValueFromDictionaryAsDouble(Expression dictionaryExpr, Expression keyExpr)
        {
            return GetValueFromDictionaryAs<double>(dictionaryExpr, keyExpr);
        }

        /// <summary>
        /// Gets the value of a given dictionary and returns it as boolean. This assumes that the key exists.
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <returns></returns>
        public static Expression GetValueFromDictionaryAsBoolean(Expression dictionaryExpr, Expression keyExpr)
        {
            return GetValueFromDictionaryAs<bool>(dictionaryExpr, keyExpr);
        }

        /// <summary>
        /// Gets the value of a given dictionary and returns it as the given type. This assumes that the key exists.
        /// This only supports the following types: int, long, decimal, float, double, bool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <returns></returns>
        private static Expression GetValueFromDictionaryAs<T>(Expression dictionaryExpr, Expression keyExpr)
        {
            var type = typeof(T);

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

        /// <summary>
        /// Gets the value of a given dictionary and returns it as datetime. This assumes that the key exists.
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the value of a given dictionary and returns it as datetime offset. This assumes that the key exists.
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the value of a given dictionary and returns it as char. Takes the first character if given a string. This assumes that the key exists.
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the value of a given dictionary and returns it as timespan. This assumes that the key exists.
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the value of a given dictionary and returns it as given enum type. This assumes that the key exists.
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the value of a given dictionary and returns it as a list of given type. This assumes that the key exists.
        /// </summary>
        /// <param name="dictionaryExpr"></param>
        /// <param name="keyExpr"></param>
        /// <param name="listType"></param>
        /// <returns></returns>
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
        }
    }
}
