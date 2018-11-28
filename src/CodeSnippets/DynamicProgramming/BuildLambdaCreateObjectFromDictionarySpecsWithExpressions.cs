using CodeSnippets.Artifacts;
using FastExpressionCompiler;
using Jil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static CodeSnippets.DynamicProgramming.ExpressionBuilder;
using CreateFunc = System.Func<System.Collections.Generic.Dictionary<string, string>, object>;

namespace CodeSnippets.DynamicProgramming
{
    public class BuildLambdaCreateObjectFromDictionarySpecsWithExpressions
    {
        public static CreateFunc Build(Type ofType)
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
            //expressionBodies.Add(variable);
            expressionBodies.Add(Expression.Assign(variable, ctor));
            

            //var lambdaGetValueFromDictionary = Expression.Lambda<Func<Dictionary<string, string>, string, string>> (GetValueFromDictionary(dictionaryParam, Expression.Parameter(typeof(string), "key"), Expression.Parameter(typeof(string))),);
            //var callLambda = lambdaGetValueFromDictionary.Body.

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
                    //case Type t when t == typeof(bool):
                    //    switch(value)
                    //    {
                    //        case "0":
                    //            propsToBind[idx] = Expression.Bind(prop, Expression.Constant(false, prop.PropertyType));
                    //            break;
                    //        case "1":
                    //            propsToBind[idx] = Expression.Bind(prop, Expression.Constant(true, prop.PropertyType));
                    //            break;
                    //        default:
                    //            if (!bool.TryParse(value, out var boolVal))
                    //                throw new Exception($"Can't convert '{value}' to boolean");
                    //            propsToBind[idx] = Expression.Bind(prop, Expression.Constant(boolVal, prop.PropertyType));
                    //            break;
                    //    }
                    //    break;
                    case Type t when t == typeof(TimeSpan):
                        expressionBodies.Add(
                            Expression.IfThen(test: ifContainsKey,
                                ifTrue: Expression.Assign(
                                        left: variableProp,
                                        right: GetValueFromDictionaryAsTimespan(dictionaryParam, propNameConst))));
                        break;
                        //case Type t when t == typeof(DateTime):
                        //    if (!DateTime.TryParse(value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out var dtValue))
                        //        throw new Exception($"Can't convert '{value}' to date/time");
                        //    propsToBind[idx] = Expression.Bind(prop, Expression.Constant(dtValue, prop.PropertyType));
                        //    break;
                        //case Type t when t == typeof(DateTimeOffset):
                        //    if (!DateTimeOffset.TryParse(value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out var dtoValue))
                        //        throw new Exception($"Can't convert '{value}' to date/time");
                        //    propsToBind[idx] = Expression.Bind(prop, Expression.Constant(dtoValue, prop.PropertyType));
                        //    break;
                        //case Type t when t == typeof(float):
                        //    if (!float.TryParse(value, out var floatVal))
                        //        throw new Exception($"Can't convert '{value}' to float");
                        //    propsToBind[idx] = Expression.Bind(prop, Expression.Constant(floatVal, prop.PropertyType));
                        //    break;
                        //case Type t when t == typeof(double):
                        //    if (!double.TryParse(value, out var doubleVal))
                        //        throw new Exception($"Can't convert '{value}' to double");
                        //    propsToBind[idx] = Expression.Bind(prop, Expression.Constant(doubleVal, prop.PropertyType));
                        //    break;
                        //case Type t when t == typeof(char) && value.Length > 0:
                        //    propsToBind[idx] = Expression.Bind(prop, Expression.Constant(value[0], prop.PropertyType));
                        //    break;
                        //case Type t when t == typeof(char) && value.Length == 0:
                        //    propsToBind[idx] = Expression.Bind(prop, Expression.Constant(char.MinValue, prop.PropertyType));
                        //    break;
                        //case Type t when t.GetGenericTypeDefinition() == typeof(List<>):
                        //    propsToBind[idx] = CreateListMemberAssignment(prop, value);
                        //    break;
                        //default:
                        //    throw new Exception("Cannot assign value to property. Type unsupported.");
                }
            }

            // add return value
            expressionBodies.Add(variable);

            //var init = Expression.MemberInit(ctor, propsToBind);
            var body = Expression.Block(expressionBodies);
            var lambda = Expression.Lambda<CreateFunc>(body, dictionaryParam);
            return lambda.CompileFast<CreateFunc>();
        }

        private static MemberAssignment CreateListMemberAssignment(PropertyInfo prop, string data)
        {
            var genericType = prop.PropertyType.GenericTypeArguments.FirstOrDefault();
            var dataConst = Expression.Constant(data, typeof(string));
            var propTypeConst = Expression.Constant(prop.PropertyType, typeof(Type));
            var optionsConst = Expression.Constant(null, typeof(Options));
            //JSON.Deserialize<object>("", Options)


            var callExpr = Expression.Call(typeof(JSON), "Deserialize",
                typeArguments: new Type[] { prop.PropertyType },
                arguments: new Expression[] { dataConst, optionsConst });

            return Expression.Bind(prop, callExpr);
        }
    }
}
