using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class CondLengthProperty : Property
    {
        private readonly CondLength condLength;

        public CondLengthProperty( CondLength condLength )
        {
            this.condLength = condLength;
        }

        public override CondLength GetCondLength()
        {
            return condLength;
        }

        public override Length GetLength()
        {
            return condLength.GetLength().GetLength();
        }

        public override object GetObject()
        {
            return condLength;
        }

        internal class Maker : PropertyMaker
        {
            public Maker( string name ) : base( name )
            {
            }
        }
    }
}