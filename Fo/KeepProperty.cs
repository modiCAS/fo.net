using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class KeepProperty : Property
    {
        private readonly Keep _keep;

        public KeepProperty( Keep keep )
        {
            this._keep = keep;
        }

        public override Keep GetKeep()
        {
            return _keep;
        }

        public override object GetObject()
        {
            return _keep;
        }

        internal class Maker : PropertyMaker
        {
            protected Maker( string name ) : base( name )
            {
            }
        }
    }
}