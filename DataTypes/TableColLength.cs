using Fonet.Fo.Expr;

namespace Fonet.DataTypes
{
    internal class TableColLength : Length
    {
        private readonly double tcolUnits;

        public TableColLength( double tcolUnits )
        {
            this.tcolUnits = tcolUnits;
        }

        public override double GetTableUnits()
        {
            return tcolUnits;
        }

        public override void ResolveTableUnit( double mpointsPerUnit )
        {
            SetComputedValue( (int)( tcolUnits * mpointsPerUnit ) );
        }

        public override string ToString()
        {
            return tcolUnits + " table-column-units";
        }

        public override Numeric AsNumeric()
        {
            return new Numeric( this );
        }
    }
}