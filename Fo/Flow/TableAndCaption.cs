using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class TableAndCaption : ToBeImplementedElement
    {
        protected TableAndCaption( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:table-and-caption";
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
            MarginProps mProps = PropMgr.GetMarginProps();
            RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();
            return base.Layout( area );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new TableAndCaption( parent, propertyList );
            }
        }
    }
}