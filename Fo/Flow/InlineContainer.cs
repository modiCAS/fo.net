using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class InlineContainer : ToBeImplementedElement
    {
        protected InlineContainer( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:inline-container";

            BorderAndPadding bap = PropMgr.GetBorderAndPadding();
            BackgroundProps bProps = PropMgr.GetBackgroundProps();
            MarginInlineProps mProps = PropMgr.GetMarginInlineProps();
            RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new InlineContainer( parent, propertyList );
            }
        }
    }
}