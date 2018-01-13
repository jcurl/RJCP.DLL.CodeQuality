namespace NUnit.Framework.HelperClasses
{
    using System;

    public class RelatedItemClassAccessor : AccessorBase
    {
        private const string AssemblyName = "NUnitExtensionsTest";
        private const string TypeName = "NUnit.Framework.HelperClasses.RelatedItemClass";
        public static PrivateType AccType = new PrivateType(AssemblyName, TypeName);

        public RelatedItemClassAccessor(string value)
            : base(AccType, new Type[] { typeof(string) }, new object[] { value }) { }

        public string Value
        {
            get
            {
                return (string)GetFieldOrProperty(nameof(Value));
            }
        }
    }

    public class RelatedCollectionClassAccessor : AccessorBase
    {
        private const string AssemblyName = "NUnitExtensionsTest";
        private const string TypeName = "NUnit.Framework.HelperClasses.RelatedCollectionClass";
        public static PrivateType AccType = new PrivateType(AssemblyName, TypeName);

        public RelatedCollectionClassAccessor() : base(AccType) { }

        public void Add(RelatedItemClassAccessor item)
        {
            Invoke(nameof(Add), new Type[] { RelatedItemClassAccessor.AccType.ReferencedType }, new object[] { item.PrivateTargetObject });
        }

        public bool IsInCollection(string value)
        {
            return (bool)Invoke(nameof(IsInCollection), value);
        }
    }
}
