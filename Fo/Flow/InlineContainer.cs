using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class InlineContainer : ToBeImplementedElement
    {
        protected InlineContainer( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:inline-container";

            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            MarginInlineProps mProps = propMgr.GetMarginInlineProps();
            RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();
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