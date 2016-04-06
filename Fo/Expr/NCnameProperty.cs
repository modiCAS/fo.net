using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class NCnameProperty : Property
    {
        private readonly string ncName;

        public NCnameProperty( string ncName )
        {
            this.ncName = ncName;
        }

        public ColorType getColor()
        {
            throw new PropertyException( "Not a Color" );
        }

        public override string GetString()
        {
            return ncName;
        }

        public override string GetNCname()
        {
            return ncName;
        }
    }
}