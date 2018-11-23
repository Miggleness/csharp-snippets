using CodeSnippets.Artifacts;
using FastExpressionCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CodeSnippets.Expressions
{
    public class ExpressionSnippets
    {
        public static Func<object> BuildLambdaCreateObjectFromDictionarySpecs(Dictionary<string, string> input, Type ofType)
        {
            var props = (from p in ofType.GetProperties()
                         from a in p.CustomAttributes
                         //where a.AttributeType == typeof(IncludeInObjectBuilderAttribute)
                         where input.Keys.Contains(p.Name)
                         select p).ToArray();

            var ctor = Expression.New(ofType);
            var propsToBind = new MemberAssignment[props.Length];

            for (int idx = 0; idx < propsToBind.Length; idx++)
            {
                var prop = props[idx];
                var value = input[prop.Name];

                switch (prop.PropertyType)
                {
                    case Type t when t == typeof(string):
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(value, prop.PropertyType));
                        break;
                    case Type t when t.IsEnum:
                        object enumVal;
                        try
                        {
                            enumVal = Enum.Parse(t, value); 
                        }
                        catch (Exception ex)
                        { 
                            throw new Exception($"'{value}' not a valid option for enum type '{t.FullName}'", ex); 
                        }
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(enumVal, prop.PropertyType));
                        break;
                    case Type t when t == typeof(int):
                        if (!int.TryParse(value, out var intVal))
                            throw new Exception($"Can't convert '{value}' to int");
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(intVal, prop.PropertyType));
                        break;
                    case Type t when t == typeof(long):
                        if (!long.TryParse(value, out var longVal))
                            throw new Exception($"Can't convert '{value}' to long");
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(longVal, prop.PropertyType));
                        break;
                    case Type t when t == typeof(decimal):
                        if (!decimal.TryParse(value, out var decVal))
                            throw new Exception($"Can't convert '{value}' to decimal");
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(decVal, prop.PropertyType));
                        break;
                    case Type t when t == typeof(TimeSpan):
                        if (!long.TryParse(value, out var ticksVal))
                            throw new Exception($"Can't convert '{value}' to long as ticks/duration");
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(new TimeSpan(ticksVal), prop.PropertyType));
                        break;
                    case Type t when t == typeof(DateTime):
                        if (!long.TryParse(value, out var ticksVal))
                            throw new Exception($"Can't convert '{value}' to long as ticks/duration");
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(new TimeSpan(ticksVal), prop.PropertyType));
                        break;
                    case Type t when t == typeof(float):
                        if (!float.TryParse(value, out var floatVal))
                            throw new Exception($"Can't convert '{value}' to float");
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(floatVal, prop.PropertyType));
                        break;
                    case Type t when t == typeof(double):
                        if (!double.TryParse(value, out var doubleVal))
                            throw new Exception($"Can't convert '{value}' to double");
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(doubleVal, prop.PropertyType));
                        break;
                    case Type t when t == typeof(char) && value.Length > 0:
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(value[0], prop.PropertyType));
                        break;
                    case Type t when t == typeof(char) && value.Length == 0:
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(char.MinValue, prop.PropertyType));
                        break;
                    
                    default:
                        throw new Exception("Cannot assign value to property. Type unsupported.");
                }
            }

            var init = Expression.MemberInit(ctor, propsToBind);
            var lambda = Expression.Lambda<Func<object>>(init);
            return lambda.CompileFast<Func<object>>();
        }
    }
}
