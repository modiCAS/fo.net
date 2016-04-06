using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class SpaceProperty : Property
    {
        private readonly Space _space;

        public SpaceProperty( Space space )
        {
            this._space = space;
        }

        public override Space GetSpace()
        {
            return _space;
        }

        public override LengthRange GetLengthRange()
        {
            return _space;
        }

        public override object GetObject()
        {
            return _space;
        }

        internal class Maker : LengthRangeProperty.Maker
        {
            protected Maker( string name ) : base( name )
            {
            }
        }
    }
}