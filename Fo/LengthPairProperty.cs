using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class LengthPairProperty : Property
    {
        private readonly LengthPair lengthPair;

        public LengthPairProperty( LengthPair lengthPair )
        {
            this.lengthPair = lengthPair;
        }

        public override LengthPair GetLengthPair()
        {
            return lengthPair;
        }

        public override object GetObject()
        {
            return lengthPair;
        }

        internal class Maker : LengthProperty.Maker
        {
            protected Maker( string name ) : base( name )
            {
            }
        }
    }
}