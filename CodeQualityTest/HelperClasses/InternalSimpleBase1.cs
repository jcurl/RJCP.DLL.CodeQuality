namespace RJCP.CodeQuality.HelperClasses
{
    internal class InternalSimpleBase1
    {
        public InternalSimpleBase1(int value)
        {
            Value = value;
        }

        public int Value { get; set; }

        public virtual string DoSomething()
        {
            return string.Format("{0}", Value);
        }
    }
}
