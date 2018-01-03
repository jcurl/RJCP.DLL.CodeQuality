namespace NUnit.Framework.HelperClasses
{
    using System;

    internal class RelatedClassTest
    {
        public RelatedClassTest(int initialValue)
        {
            Value = initialValue;
        }

        public int Value { get; private set; }
    }

    internal class RelatedClassTestFactory
    {
        public RelatedClassTestFactory() { }

        public RelatedClassTest Create()
        {
            return new RelatedClassTest(42);
        }
    }
}
