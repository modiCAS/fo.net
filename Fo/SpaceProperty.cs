using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class SpaceProperty : Property
    {
        private readonly Space space;

        public SpaceProperty( Space space )
        {
            this.space = space;
        }

        public override Space GetSpace()
        {
            return space;
        }

        public override LengthRange GetLengthRange()
        {
            return space;
        }

        public override object GetObject()
        {
            return space;
        }

        internal class Maker : LengthRangeProperty.Maker
        {
            protected Maker( string name ) : base( name )
            {
            }
        }
    }
}