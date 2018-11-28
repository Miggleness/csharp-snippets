using CodeSnippets.Artifacts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CreateFunc = System.Func<System.Collections.Generic.Dictionary<string, string>, object>;

namespace CodeSnippets.DynamicProgramming
{
    public class BuildLambdaCreateObjectFromDictionarySpecsScriptingApi
    {
        public static CreateFunc Build()
        {
            var code = @"
using System;
using System.Collections.Generic;

public class WorkflowDataFactory
{
    public string Do(Dictionary<string, string> input)
    {
        if(input.ContainsKey(""Value1\""))
        {
            return ""YAY"";
        }
        return ""NAY"";
    }
}
";

            var tree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { tree }, references: new[] {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Dictionary<,>).Assembly.Location)
                });
            compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { tree }, references: new[] {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Dictionary<,>).Assembly.Location)
                });
            compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { tree }, references: new[] {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Dictionary<,>).Assembly.Location)
                });

            //Emit to stream
            var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);

            //Load into currently running assembly. Normally we'd probably
            //want to do this in an AppDomain
            var ourAssembly = Assembly.Load(ms.ToArray());
            var type = ourAssembly.GetType("WorkflowDataFactory");
            var o = (dynamic)Activator.CreateInstance(type);
            return (CreateFunc)o.Do;


            //    var props = (from p in type.GetProperties()
            //                 from a in p.CustomAttributes
            //                 where a.AttributeType == typeof(IncludeInObjectBuilderAttribute)
            //                 where input.Keys.Contains(p.Name)
            //                 select p);

            //    var options = ScriptOptions.Default.AddReferences(type.Assembly)
            //        .AddImports(type.Namespace);


            //    var codeBuilder = new StringBuilder()
            //        .Append("()=> new ")
            //        .Append(type.Name)
            //        .Append("(){");

            //    foreach (var p in props)
            //    {
            //        codeBuilder.Append(p.Name)
            //            .Append("=");

            //        switch (p.PropertyType)
            //        {
            //            case Type t when t == typeof(string):
            //                codeBuilder.Append("\"").Append(input[p.Name]).Append("\"");
            //                break;
            //            case Type t when t == typeof(char):
            //                codeBuilder.Append("'").Append(input[p.Name]).Append("'");
            //                break;
            //            default:
            //                codeBuilder.Append(input[p.Name]);
            //                break;
            //        }
            //        codeBuilder.Append(",");

            //    }
            //    codeBuilder.Append("}");
            //    var code = codeBuilder.ToString();

            //    return CSharpScript.EvaluateAsync<Func<object>>(code, options).GetAwaiter().GetResult();
        }
    }
}
