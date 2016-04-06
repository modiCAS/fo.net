using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class NCnameProperty : Property
    {
        private readonly string _ncName;

        public NCnameProperty( string ncName )
        {
            this._ncName = ncName;
        }

        public ColorType GetColor()
        {
            throw new PropertyException( "Not a Color" );
        }

        public override string GetString()
        {
            return _ncName;
        }

        public override string GetNCname()
        {
            return _ncName;
        }
    }
}