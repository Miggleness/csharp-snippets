using CodeSnippets.Artifacts;
using FastExpressionCompiler;
using Jil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using CreateFunc = System.Func<System.Collections.Generic.Dictionary<string, string>, object>;

namespace CodeSnippets.DynamicProgramming
{
    public class BuildLambdaCreateObjectFromDictionarySpecsWithExpressions
    {
        public static CreateFunc BuildLambdaCreateObjectFromDictionarySpecs(Type ofType)
        {
            var props = (from p in ofType.GetProperties()
                         from a in p.CustomAttributes
                         where a.AttributeType == typeof(IncludeInObjectBuilderAttribute)
                         select p).ToArray();

            var ctor = Expression.New(ofType);
            var propsToBind = new List<MemberAssignment>();
            var dictionaryParam = Expression.Parameter(typeof(Dictionary<string, string>), "inputDictionary");
            //var lambdaGetValueFromDictionary = Expression.Lambda<Func<Dictionary<string, string>, string, string>> (GetValueFromDictionary(dictionaryParam, Expression.Parameter(typeof(string), "key"), Expression.Parameter(typeof(string))),);
            //var callLambda = lambdaGetValueFromDictionary.Body.

            for (int idx = 0; idx < props.Length; idx++)
            {
                var prop = props[idx];
                var propNameConst = Expression.Constant(prop.Name);

                switch (prop.PropertyType)
                {
                    case Type t when t == typeof(string):

                        //var containsKey = DictionaryContainsKey(dictionaryParam, propNameConst);

                        //var assign = Expression.Assign(prop, GetValueFromDictionaryAsString(dictionaryParam, propNameConst, typeof(string)));
                        //var ifThen = Expression.IfThen(containsKey, assign);


                        //Expression.Bind(
                        //        prop,
                        //        GetValueFromDictionary(dictionaryParam, propNameConst, Expression.Parameter(typeof(string))))



                        //propsToBind.Add(
                        //    Expression.Bind(
                        //        prop, 
                        //        GetValueFromDictionary(dictionaryParam, propNameConst, Expression.Parameter(typeof(string))))
                        //);
                        break;
                        //case Type t when t.IsEnum:
                        //    object enumVal;
                        //    try
                        //    {
                        //        enumVal = Enum.Parse(t, value); 
                        //    }
                        //    catch (Exception ex)
                        //    { 
                        //        throw new Exception($"'{value}' not a valid option for enum type '{t.FullName}'", ex); 
                        //    }
                        //    propsToBind[idx] = Expression.Bind(prop, Expression.Constant(enumVal, prop.PropertyType));
                        //    break;
                        //case Type t when t == typeof(int):
                        //    if (!int.TryParse(value, out var intVal))
                        //        throw new Exception($"Can't convert '{value}' to int");
                        //    propsToBind[idx] = Expression.Bind(prop, Expression.Constant(intVal, prop.PropertyType));
                        //    break;
                        //case Type t when t == typeof(long):
                        //    if (!long.TryParse(value, out var longVal))
                        //        throw new Exception($"Can't convert '{value}' to long");
                        //    propsToBind[idx] = Expression.Bind(prop, Expression.Constant(longVal, prop.PropertyType));
                        //    break;
                        //case Type t when t == typeof(decimal):
                        //    if (!decimal.TryParse(value, out var decVal))
                        //        throw new Exception($"Can't convert '{value}' to decimal");
                        //    propsToBind[idx] = Expression.Bind(prop, Expression.Constant(decVal, prop.PropertyType));
                        //    break;
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
                        //case Type t when t == typeof(TimeSpan):
                        //    if (!long.TryParse(value, out var ticksVal))
                        //        throw new Exception($"Can't convert '{value}' to long as ticks/duration");
                        //    propsToBind[idx] = Expression.Bind(prop, Expression.Constant(new TimeSpan(ticksVal), prop.PropertyType));
                        //    break;
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

            var init = Expression.MemberInit(ctor, propsToBind);
            var lambda = Expression.Lambda<CreateFunc>(init, dictionaryParam);
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
