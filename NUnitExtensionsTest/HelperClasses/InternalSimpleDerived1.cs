namespace NUnit.Framework.HelperClasses
{
    using System;

    internal class InternalSimpleDerived1 : InternalSimpleBase1
    {
        public InternalSimpleDerived1(int value, string description) : base(value)
        {
            Description = description;
        }

        public string Description { get; set; }

        public override string DoSomething()
        {
            return string.Format("{0}: {1}", Description, Value);
        }
    }
}
