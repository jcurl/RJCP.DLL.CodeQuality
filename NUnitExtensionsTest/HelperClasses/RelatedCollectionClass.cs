namespace NUnit.Framework.HelperClasses
{
    using System;
    using System.Collections.Generic;

    internal class RelatedItemClass
    {
        public RelatedItemClass(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }

    internal class RelatedCollectionClass
    {
        private HashSet<string> m_Set = new HashSet<string>();

        public void Add(RelatedItemClass item)
        {
            if (m_Set.Contains(item.Value)) {
                throw new ArgumentException("Item already in collection", nameof(item));
            }
            m_Set.Add(item.Value);
        }

        public bool IsInCollection(string value)
        {
            return m_Set.Contains(value);
        }
    }
}
