namespace Fonet.Fo
{
    internal class ColorProfile : ToBeImplementedElement
    {
        protected ColorProfile( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:color-profile";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new ColorProfile( parent, propertyList );
            }
        }
    }
}