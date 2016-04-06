namespace Fonet.Fo
{
    internal class Declarations : ToBeImplementedElement
    {
        protected Declarations( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:declarations";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Declarations( parent, propertyList );
            }
        }
    }
}