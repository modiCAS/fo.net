using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class InitialPropertySet : ToBeImplementedElement
    {
        protected InitialPropertySet( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:initial-property-set";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            AccessibilityProps mAccProps = PropMgr.GetAccessibilityProps();
            AuralProps mAurProps = PropMgr.GetAuralProps();
            BorderAndPadding bap = PropMgr.GetBorderAndPadding();
            BackgroundProps bProps = PropMgr.GetBackgroundProps();
            RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();

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