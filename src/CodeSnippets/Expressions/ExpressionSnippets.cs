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
                            where a.AttributeType == typeof(IncludeInObjectBuilderAttribute)
                            where input.Keys.Contains(p.Name)
                            select p).ToArray();

            var ctor = Expression.New(ofType);
            var propsToBind = new MemberAssignment[props.Length];

            for(int idx=0;idx<propsToBind.Length;idx++)
            {
                var prop = props[idx];

                switch(prop.PropertyType)
                {
                    case Type t when t.IsEnum:
                        object enumVal;
                        try 
                            { enumVal = Enum.Parse(t, input[prop.Name]); }
                        catch(Exception ex)
                            { throw new Exception($"Can't convert '{input[prop.Name]}' into a value of enum type '{t.FullName}'", ex); }
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(enumVal, prop.PropertyType));
                        break;
                    case Type t when t==typeof(int):
                        if(!int.TryParse(input[prop.Name], out var intVal))
                            { throw new Exception($"Can't convert '{input[prop.Name]}' to int"); }
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(intVal, prop.PropertyType));
                        break;
                    default:
                        propsToBind[idx] = Expression.Bind(prop, Expression.Constant(input[prop.Name], prop.PropertyType));
                        break;
                }
            }

            var init = Expression.MemberInit(ctor, propsToBind);
            var lambda = Expression.Lambda<Func<object>>(init);
            return lambda.CompileFast<Func<object>>();
            }
        }
}
