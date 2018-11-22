using CodeSnippets.Artifacts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeSnippets.Tests
{
    public class MyInstantiatedClass
    {
        [IncludeInObjectBuilder("Whatever")]
        public int Id { get; set; }
        [IncludeInObjectBuilder("Whatever")]
        public MyInstantiatedClassEnum Type { get; set; }
    }

    public enum MyInstantiatedClassEnum
    {
        Val1,
        Val2
    }
}
