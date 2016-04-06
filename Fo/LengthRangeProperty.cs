using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class LengthRangeProperty : Property
    {
        private readonly LengthRange _lengthRange;

        public LengthRangeProperty( LengthRange lengthRange )
        {
            this._lengthRange = lengthRange;
        }

        public override LengthRange GetLengthRange()
        {
            return _lengthRange;
        }

        public override object GetObject()
        {
            return _lengthRange;
        }

        internal class Maker : LengthProperty.Maker
        {
            protected Maker( string name ) : base( name )
            {
            }
        }
    }
}