using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Float : ToBeImplementedElement
    {
        protected Float( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:float";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            return base.Layout( area );
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