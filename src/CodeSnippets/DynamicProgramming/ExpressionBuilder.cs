using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CodeSnippets.DynamicProgramming
{
    public static class ExpressionBuilder
    {
        public static Expression DictionaryContainsKey(Expression dictionaryExpr, Expression keyExpr)
        {
            return Expression.Call(dictionaryExpr, "ContainsKey", null, keyExpr);
        }

        public static Expression GetValueFromDictionaryAsString(Expression dictionaryExpr, Expression keyExpr, Type resultType)
        {
            var resultExpr = Expression.Variable(resultType);
            var assignExpression = Expression.Assign(resultExpr, Expression.Property(dictionaryExpr, "Item", keyExpr));
            return Expression.Block(new[] { resultExpr },
                assignExpression,
                resultExpr);
        }

        //public static Expression AssignDictionaryValueIfKeyExists(
        //    Expression dictionaryExpr,
        //    Expression keyExpr,
        //    Type resultType)
        //{
        //    var result = Expression.Parameter(resultType);
        //    var defaultValue = Expression.Default(resultType);

        //    return Expression.Block(
        //        Expression.IfThenElse(
        //            test: DictionaryContainsKey(dictionaryExpr, keyExpr),
        //            ifTrue: Expression.Assign(result, 
        //        );


        //}

    }
}
