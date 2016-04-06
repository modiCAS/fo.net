using Fonet.Fo.Expr;

namespace Fonet.DataTypes
{
    internal class PercentLength : Length
    {
        private readonly double factor;

        public PercentLength( double factor ) : this( factor, null )
        {
        }

        public PercentLength( double factor, IPercentBase lbase )
        {
            this.factor = factor;
            BaseLength = lbase;
        }

        public IPercentBase BaseLength { get; set; }

        public override void ComputeValue()
        {
            SetComputedValue( (int)( factor * BaseLength.GetBaseLength() ) );
        }

        public double value()
        {
            return factor;
        }

        public override string ToString()
        {
            return factor * 100.0 + "%";
        }

        public override Numeric AsNumeric()
        {
            return new Numeric( this );
        }
    }
}