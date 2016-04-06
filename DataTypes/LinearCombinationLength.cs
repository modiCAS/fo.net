using System.Collections;

namespace Fonet.DataTypes
{
    internal class LinearCombinationLength : Length
    {
        protected ArrayList factors;
        protected ArrayList lengths;

        public LinearCombinationLength()
        {
            factors = new ArrayList();
            lengths = new ArrayList();
        }

        public void AddTerm( double factor, Length length )
        {
            factors.Add( factor );
            lengths.Add( length );
        }

        public override void ComputeValue()
        {
            var result = 0;
            int numFactors = factors.Count;
            for ( var i = 0; i < numFactors; ++i )
            {
                var d = (double)factors[ i ];
                var l = (Length)lengths[ i ];
                result += (int)( d * l.MValue() );
            }
            SetComputedValue( result );
        }
    }
}