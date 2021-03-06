namespace Fonet.Fo.Flow
{
    internal class MultiPropertySet : ToBeImplementedElement
    {
        protected MultiPropertySet( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:multi-property-set";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new MultiPropertySet( parent, propertyList );
            }
        }
    }
}