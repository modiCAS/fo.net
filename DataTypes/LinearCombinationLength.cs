using System.Collections;

namespace Fonet.DataTypes
{
    internal class LinearCombinationLength : Length
    {
        protected ArrayList Factors;
        protected ArrayList Lengths;

        public LinearCombinationLength()
        {
            Factors = new ArrayList();
            Lengths = new ArrayList();
        }

        public void AddTerm( double factor, Length length )
        {
            Factors.Add( factor );
            Lengths.Add( length );
        }

        public override void ComputeValue()
        {
            var result = 0;
            int numFactors = Factors.Count;
            for ( var i = 0; i < numFactors; ++i )
            {
                var d = (double)Factors[ i ];
                var l = (Length)Lengths[ i ];
                result += (int)( d * l.MValue() );
            }
            SetComputedValue( result );
        }
    }
}