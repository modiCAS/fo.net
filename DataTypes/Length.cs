using Fonet.Fo.Expr;

namespace Fonet.DataTypes
{
    internal class Length
    {
        private bool _bIsComputed;
        private int _millipoints;

        public int MValue()
        {
            if ( !_bIsComputed )
                ComputeValue();
            return _millipoints;
        }

        public virtual void ComputeValue()
        {
        }

        protected void SetComputedValue( int millipoints, bool bSetComputed = true )
        {
            _millipoints = millipoints;
            _bIsComputed = bSetComputed;
        }

        public virtual bool IsAuto()
        {
            return false;
        }

        public bool IsComputed()
        {
            return _bIsComputed;
        }

        public virtual double GetTableUnits()
        {
            return 0.0;
        }

        public virtual void ResolveTableUnit( double dTableUnit )
        {
        }

        public virtual Numeric AsNumeric()
        {
            return null;
        }

        public override string ToString()
        {
            return _millipoints + "mpt";
        }
    }
}