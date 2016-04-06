using Fonet.Fo;

namespace Fonet.DataTypes
{
    internal class LengthRange : ICompoundDatatype
    {
        private const int MINSET = 1;
        private const int OPTSET = 2;
        private const int MAXSET = 4;
        private bool bChecked;
        private Property maximum;
        private Property minimum;
        private Property optimum;

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
            this.minimum = minimum;
            if ( !bIsDefault )
                BfSet |= MINSET;
        }

        protected void SetMaximum( Property max, bool bIsDefault )
        {
            maximum = max;
            if ( !bIsDefault )
                BfSet |= MAXSET;
        }

        protected void SetOptimum( Property opt, bool bIsDefault )
        {
            optimum = opt;
            if ( !bIsDefault )
                BfSet |= OPTSET;
        }

        private void CheckConsistency()
        {
            if ( bChecked )
                return;
            bChecked = true;
        }

        public Property GetMinimum()
        {
            CheckConsistency();
            return minimum;
        }

        public Property GetMaximum()
        {
            CheckConsistency();
            return maximum;
        }

        public Property GetOptimum()
        {
            CheckConsistency();
            return optimum;
        }
    }
}