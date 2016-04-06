using Fonet.Fo.Flow;

namespace Fonet.Fo.Expr
{
    internal class BodyStartFunction : FunctionBase
    {
        public override int NumArgs
        {
            get { return 0; }
        }

        public override Property Eval( Property[] args, PropertyInfo pInfo )
        {
            Numeric distance = pInfo.GetPropertyList().GetProperty( "provisional-distance-between-starts" ).GetNumeric();

            FObj item = pInfo.GetFo();
            while ( item != null && !( item is ListItem ) )
                item = item.GetParent();
            if ( item == null )
                throw new PropertyException( "body-start() called from outside an fo:list-item" );

            Numeric startIndent =
                item.Properties.GetProperty( "start-indent" ).GetNumeric();

            return new NumericProperty( distance.Add( startIndent ) );
        }
    }
}