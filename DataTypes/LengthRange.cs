using Fonet.Fo;

namespace Fonet.DataTypes
{
    internal class LengthRange : ICompoundDatatype
    {
        private const int Minset = 1;
        private const int Optset = 2;
        private const int Maxset = 4;
        private bool _bChecked;
        private Property _maximum;
        private Property _minimum;
        private Property _optimum;

        public LengthRange()
        {
            BfSet = 0;
        }

        public int BfSet { get; private set; }

        public virtual void SetComponent( string sCmpnName, Property cmpnValue,
            bool bIsDefault )
        {
            if ( sCmpnName.Equals( "minimum" ) )
                SetMinimum( cmpnValue, bIsDefault );
            else if ( sCmpnName.Equals( "optimum" ) )
                SetOptimum( cmpnValue, bIsDefault );
            else if ( sCmpnName.Equals( "maximum" ) )
                SetMaximum( cmpnValue, bIsDefault );
        }

        public virtual Property GetComponent( string sCmpnName )
        {
            if ( sCmpnName.Equals( "minimum" ) )
                return GetMinimum();
            if ( sCmpnName.Equals( "optimum" ) )
                return GetOptimum();
            if ( sCmpnName.Equals( "maximum" ) )
                return GetMaximum();
            return null;
        }

        protected void SetMinimum( Property minimum, bool bIsDefault )
        {
            this._minimum = minimum;
            if ( !bIsDefault )
                BfSet |= Minset;
        }

        protected void SetMaximum( Property max, bool bIsDefault )
        {
            _maximum = max;
            if ( !bIsDefault )
                BfSet |= Maxset;
        }

        protected void SetOptimum( Property opt, bool bIsDefault )
        {
            _optimum = opt;
            if ( !bIsDefault )
                BfSet |= Optset;
        }

        private void CheckConsistency()
        {
            if ( _bChecked )
                return;
            _bChecked = true;
        }

        public Property GetMinimum()
        {
            CheckConsistency();
            return _minimum;
        }

        public Property GetMaximum()
        {
            CheckConsistency();
            return _maximum;
        }

        public Property GetOptimum()
        {
            CheckConsistency();
            return _optimum;
        }
    }
}