using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class BidiOverride : ToBeImplementedElement
    {
        protected BidiOverride( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:bidi-override";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            AuralProps mAurProps = propMgr.GetAuralProps();
            RelativePositionProps mProps = propMgr.GetRelativePositionProps();
            return base.Layout( area );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new BidiOverride( parent, propertyList );
            }
        }
    }
}