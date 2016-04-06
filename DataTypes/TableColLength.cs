using Fonet.Fo.Expr;

namespace Fonet.DataTypes
{
    internal class TableColLength : Length
    {
        private readonly double _tcolUnits;

        public TableColLength( double tcolUnits )
        {
            this._tcolUnits = tcolUnits;
        }

        public override double GetTableUnits()
        {
            return _tcolUnits;
        }

        public override void ResolveTableUnit( double mpointsPerUnit )
        {
            SetComputedValue( (int)( _tcolUnits * mpointsPerUnit ) );
        }

        public override string ToString()
        {
            return _tcolUnits + " table-column-units";
        }

        public override Numeric AsNumeric()
        {
            return new Numeric( this );
        }
    }
}