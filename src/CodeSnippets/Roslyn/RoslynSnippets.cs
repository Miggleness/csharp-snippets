using CodeSnippets.Artifacts;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSnippets.Roslyn
{
    public class RoslynSnippets
    {
        public static Func<object> BuildLambdaCreateObjectFromDictionarySpecs(Dictionary<string, string> input, Type type)
        {
            var props = (from p in type.GetProperties()
                         from a in p.CustomAttributes
                         where a.AttributeType == typeof(IncludeInObjectBuilderAttribute)
                         where input.Keys.Contains(p.Name)
                         select p);

            var options = ScriptOptions.Default.AddReferences(type.Assembly)
                .AddImports(type.Namespace);


            var codeBuilder = new StringBuilder()
                .Append("()=> new ")
                .Append(type.Name)
                .Append("(){");

            foreach (var p in props)
            {
                codeBuilder.Append(p.Name)
                    .Append("=");

                switch (p.PropertyType)
                {
                    case Type t when t == typeof(string):
                        codeBuilder.Append("\"").Append(input[p.Name]).Append("\"");
                        break;
                    case Type t when t == typeof(char):
                        codeBuilder.Append("'").Append(input[p.Name]).Append("'");
                        break;
                    default:
                        codeBuilder.Append(input[p.Name]);
                        break;
                }
                codeBuilder.Append(",");

            }
            codeBuilder.Append("}");
            var code = codeBuilder.ToString();

            return CSharpScript.EvaluateAsync<Func<object>>(code, options).GetAwaiter().GetResult();
        }
    }
}
