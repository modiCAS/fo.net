using Fonet.DataTypes;
using Fonet.Fo.Flow;

namespace Fonet.Fo.Expr
{
    internal class LabelEndFunction : FunctionBase
    {
        public override int NumArgs
        {
            get { return 0; }
        }

        public override Property Eval( Property[] args, PropertyInfo pInfo )
        {
            Length distance =
                pInfo.GetPropertyList().GetProperty( "provisional-distance-between-starts" ).GetLength();
            Length separation =
                pInfo.GetPropertyList().GetNearestSpecifiedProperty( "provisional-label-separation" ).GetLength();

            FObj item = pInfo.GetFo();
            while ( item != null && !( item is ListItem ) )
                item = item.GetParent();
            if ( item == null )
                throw new PropertyException( "label-end() called from outside an fo:list-item" );
            Length startIndent = item.Properties.GetProperty( "start-indent" ).GetLength();

            var labelEnd = new LinearCombinationLength();

            var bse = new LengthBase( item, pInfo.GetPropertyList(),
                LengthBase.ContainingBox );
            var refWidth = new PercentLength( 1.0, bse );

            labelEnd.AddTerm( 1.0, refWidth );
            labelEnd.AddTerm( -1.0, distance );
            labelEnd.AddTerm( -1.0, startIndent );
            labelEnd.AddTerm( 1.0, separation );

            labelEnd.ComputeValue();

            return new LengthProperty( labelEnd );
        }
    }
}