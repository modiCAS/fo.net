using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class LengthPairProperty : Property
    {
        private readonly LengthPair _lengthPair;

        public LengthPairProperty( LengthPair lengthPair )
        {
            this._lengthPair = lengthPair;
        }

        public override LengthPair GetLengthPair()
        {
            return _lengthPair;
        }

        public override object GetObject()
        {
            return _lengthPair;
        }

        internal class Maker : LengthProperty.Maker
        {
            protected Maker( string name ) : base( name )
            {
            }
        }
    }
}