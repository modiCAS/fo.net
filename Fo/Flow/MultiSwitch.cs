using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class MultiSwitch : ToBeImplementedElement
    {
        protected MultiSwitch( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:multi-switch";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            return base.Layout( area );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new MultiSwitch( parent, propertyList );
            }
        }
    }
}