using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class KeepProperty : Property
    {
        private readonly Keep keep;

        public KeepProperty( Keep keep )
        {
            this.keep = keep;
        }

        public override Keep GetKeep()
        {
            return keep;
        }

        public override object GetObject()
        {
            return keep;
        }

        internal class Maker : PropertyMaker
        {
            protected Maker( string name ) : base( name )
            {
            }
        }
    }
}