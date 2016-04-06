namespace Fonet.Fo.Flow
{
    internal class Float : ToBeImplementedElement
    {
        protected Float( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:float";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Float( parent, propertyList );
            }
        }
    }
}