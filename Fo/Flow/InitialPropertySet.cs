using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class InitialPropertySet : ToBeImplementedElement
    {
        protected InitialPropertySet( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:initial-property-set";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            AuralProps mAurProps = propMgr.GetAuralProps();
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

            return base.Layout( area );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new InitialPropertySet( parent, propertyList );
            }
        }
    }
}