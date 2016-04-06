using Fonet.Fo.Expr;

namespace Fonet.DataTypes
{
    internal class Length
    {
        protected bool BIsComputed;
        protected int Millipoints;

        public int MValue()
        {
            if ( !BIsComputed )
                ComputeValue();
            return Millipoints;
        }

        public virtual void ComputeValue()
        {
        }

        protected void SetComputedValue( int millipoints )
        {
            SetComputedValue( millipoints, true );
        }

        protected void SetComputedValue( int millipoints, bool bSetComputed )
        {
            this.Millipoints = millipoints;
            BIsComputed = bSetComputed;
        }

        public virtual bool IsAuto()
        {
            return false;
        }

        public bool IsComputed()
        {
            return BIsComputed;
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
            return Millipoints + "mpt";
        }
    }
}