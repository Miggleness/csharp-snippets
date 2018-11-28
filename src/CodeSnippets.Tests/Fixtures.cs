using CodeSnippets.Artifacts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeSnippets.Tests
{
    public class MyInstantiatedClass
    {
        [IncludeInObjectBuilder("whatever")]
        public int Id { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public long LongId { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public string Name { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public MyInstantiatedClassEnum Type { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public decimal Money { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public TimeSpan OffsetBy { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public DateTime Birthday { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public DateTimeOffset DDay { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public float Approximation { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public double LargerApproximation { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public char EmptyChar { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public char OneChar { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public bool BoolText { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public bool BoolNum { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public List<string> StringList { get; set; }
        [IncludeInObjectBuilder("whatever")]
        public List<int> IntList { get; set; }
    }

    public class MyInstantiatedClassWithObjectProperty
    {
        [IncludeInObjectBuilder("whatever")]
        public object Obj { get; set; }
    }

    public enum MyInstantiatedClassEnum
    {
        Val1,
        Val2
    }
}

