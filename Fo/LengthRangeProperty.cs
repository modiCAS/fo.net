using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class LengthRangeProperty : Property
    {
        private readonly LengthRange lengthRange;

        public LengthRangeProperty( LengthRange lengthRange )
        {
            this.lengthRange = lengthRange;
        }

        public override LengthRange GetLengthRange()
        {
            return lengthRange;
        }

        public override object GetObject()
        {
            return lengthRange;
        }

        internal class Maker : LengthProperty.Maker
        {
            protected Maker( string name ) : base( name )
            {
            }
        }
    }
}