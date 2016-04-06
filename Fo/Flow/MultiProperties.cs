using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class MultiProperties : ToBeImplementedElement
    {
        protected MultiProperties( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:multi-properties";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            AccessibilityProps mAccProps = PropMgr.GetAccessibilityProps();
            return base.Layout( area );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new MultiProperties( parent, propertyList );
            }
        }
    }
}