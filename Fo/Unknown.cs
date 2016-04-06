using Fonet.Layout;

namespace Fonet.Fo
{
    internal class Unknown : FObj
    {
        protected Unknown( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "unknown";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            return new Status( Status.Ok );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Unknown( parent, propertyList );
            }
        }
    }
}