using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectFactory
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class IncludeInObjectBuilderAttribute : Attribute
    {
        readonly string name;

        // This is a positional argument
        public IncludeInObjectBuilderAttribute(string name)
        {
            this.name = name;

            // TODO: Implement code here

            throw new NotImplementedException();
        }

        public string Name
        {
            get { return name; }
        }
    }
}
