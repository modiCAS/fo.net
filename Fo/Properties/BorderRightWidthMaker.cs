using System.Text;

namespace Fonet.Fo.Properties
{
    internal class BorderRightWidthMaker : GenericBorderWidth
    {
        protected BorderRightWidthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderRightWidthMaker( propName );
        }


        public override Property Compute( PropertyList propertyList )
        {
            FObj parentFo = propertyList.GetParentFObj();
            var sbExpr = new StringBuilder();
            Property p = null;
            sbExpr.Append( "border-" );
            sbExpr.Append( propertyList.WmAbsToRel( PropertyList.Right ) );
            sbExpr.Append( "-width" );
            p = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( p != null )
                p = ConvertProperty( p, propertyList, parentFo );

            return p;
        }

        public override Property GetShorthand( PropertyList propertyList )
        {
            Property p = null;
            ListProperty listprop;

            if ( p == null )
            {
                listprop = (ListProperty)propertyList.GetExplicitProperty( "border-right" );
                if ( listprop != null )
                {
                    IShorthandParser shparser = new GenericShorthandParser( listprop );
                    p = shparser.GetValueForProperty( PropName, this, propertyList );
                }
            }

            if ( p == null )
            {
                listprop = (ListProperty)propertyList.GetExplicitProperty( "border-width" );
                if ( listprop != null )
                {
                    IShorthandParser shparser = new BoxPropShorthandParser( listprop );
                    p = shparser.GetValueForProperty( PropName, this, propertyList );
                }
            }

            if ( p == null )
            {
                listprop = (ListProperty)propertyList.GetExplicitProperty( "border" );
                if ( listprop != null )
                {
                    IShorthandParser shparser = new GenericShorthandParser( listprop );
                    p = shparser.GetValueForProperty( PropName, this, propertyList );
                }
            }

            return p;
        }
    }
}