using FastExpressionCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static ObjectFactory.ExpressionBuilder;
using ObjectFactoryFunc = System.Func<System.Collections.Generic.Dictionary<string, string>, object>;

namespace ObjectFactory
{
    public class ObjectFactoryBuilderWithExpressions
    {
        public static Expression ExpressionFor<T>()
        {
            return ExpressionForInternal(typeof(T));
        }

        public static ObjectFactoryFunc For<T>()
        {
            var lambda = ExpressionForInternal(typeof(T));
            return lambda.CompileFast<ObjectFactoryFunc>();
        }

        private static Expression<ObjectFactoryFunc> ExpressionForInternal(Type ofType)
        {
            var props = (from p in ofType.GetProperties()
                         from a in p.CustomAttributes
                         where a.AttributeType == typeof(IncludeInObjectBuilderAttribute)
                         select p).ToArray();
            
            var propsToBind = new List<MemberAssignment>();
            var dictionaryParam = Expression.Parameter(typeof(Dictionary<string, string>), "inputDictionary");
            var expressionBodies = new List<Expression>();

            var ctor = Expression.New(ofType);
            var variable = Expression.Variable(ofType, "result");
            
            expressionBodies.Add(Expression.Assign(variable, ctor));            

            for (int idx = 0; idx < props.Length; idx++)
            {
                var prop = props[idx];
                var propNameConst = Expression.Constant(prop.Name);
                var variableProp = Expression.Property(variable, prop);
                var ifContainsKey = DictionaryContainsKeyExpression(dictionaryParam, propNameConst);

                switch (prop.PropertyType)
                {
                    case Type t when t == typeof(string):
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey, 
                                ifTrue: Expression.Assign(
                                        left: variableProp, 
                                        right: GetValueFromDictionaryAsString(dictionaryParam, propNameConst))));
                        break;
                    case Type t when t.IsEnum:
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey,
                                ifTrue: Expression.Assign(
                                        left: variableProp,
                                        right: GetValueFromDictionaryAsEnum(dictionaryParam, propNameConst, prop.PropertyType))));
                        break;
                    case Type t when t == typeof(int):
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey,
                                ifTrue: Expression.Assign(
                                        left: variableProp,
                                        right: GetValueFromDictionaryAsInt(dictionaryParam, propNameConst))));
                        break;
                    case Type t when t == typeof(long):
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey,
                                ifTrue: Expression.Assign(
                                        left: variableProp,
                                        right: GetValueFromDictionaryAsLong(dictionaryParam, propNameConst))));
                        break;
                    case Type t when t == typeof(decimal):
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey,
                                ifTrue: Expression.Assign(
                                        left: variableProp,
                                        right: GetValueFromDictionaryAsDecimal(dictionaryParam, propNameConst))));
                        break;
                    case Type t when t == typeof(bool):
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey,
                                ifTrue: Expression.Assign(
                                        left: variableProp,
                                        right: GetValueFromDictionaryAsBoolean(dictionaryParam, propNameConst))));
                        break;
                    case Type t when t == typeof(TimeSpan):
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey,
                                ifTrue: Expression.Assign(
                                        left: variableProp,
                                        right: GetValueFromDictionaryAsTimespan(dictionaryParam, propNameConst))));
                        break;
                    case Type t when t == typeof(DateTime):
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey,
                                ifTrue: Expression.Assign(
                                        left: variableProp,
                                        right: GetValueFromDictionaryAsDateTime(dictionaryParam, propNameConst))));
                        break;
                    case Type t when t == typeof(DateTimeOffset):
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey,
                                ifTrue: Expression.Assign(
                                        left: variableProp,
                                        right: GetValueFromDictionaryAsDateTimeOffset(dictionaryParam, propNameConst))));
                        break;
                    case Type t when t == typeof(float):
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey,
                                ifTrue: Expression.Assign(
                                        left: variableProp,
                                        right: GetValueFromDictionaryAsFloat(dictionaryParam, propNameConst))));
                        break;
                    case Type t when t == typeof(double):
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey,
                                ifTrue: Expression.Assign(
                                        left: variableProp,
                                        right: GetValueFromDictionaryAsDouble(dictionaryParam, propNameConst))));
                        break;
                    case Type t when t == typeof(char):
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey,
                                ifTrue: Expression.Assign(
                                        left: variableProp,
                                        right: GetValueFromDictionaryAsChar(dictionaryParam, propNameConst))));
                        break;
                    case Type t when t.GetGenericTypeDefinition() == typeof(List<>):
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey,
                                ifTrue: Expression.Assign(
                                        left: variableProp,
                                        right: GetValueFromDictionaryAsList(dictionaryParam, propNameConst, t))));
                        break;
                    default:
                        throw new Exception("Cannot assign value to property. Type unsupported.");
                }
            }

            // add return value
            expressionBodies.Add(variable);

            var body = Expression.Block(expressionBodies);
            return Expression.Lambda<ObjectFactoryFunc>(body, dictionaryParam);

        }
    }
}
