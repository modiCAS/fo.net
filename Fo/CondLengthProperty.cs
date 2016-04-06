using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class CondLengthProperty : Property
    {
        private readonly CondLength _condLength;

        public CondLengthProperty( CondLength condLength )
        {
            this._condLength = condLength;
        }

        public override CondLength GetCondLength()
        {
            return _condLength;
        }

        public override Length GetLength()
        {
            return _condLength.GetLength().GetLength();
        }

        public override object GetObject()
        {
            return _condLength;
        }

        internal class Maker : PropertyMaker
        {
            public Maker( string name ) : base( name )
            {
            }
        }
    }
}