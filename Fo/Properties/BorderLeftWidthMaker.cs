using System.Text;

namespace Fonet.Fo.Properties
{
    internal class BorderLeftWidthMaker : GenericBorderWidth
    {
        protected BorderLeftWidthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderLeftWidthMaker( propName );
        }


        public override Property Compute( PropertyList propertyList )
        {
            FObj parentFo = propertyList.GetParentFObj();
            var sbExpr = new StringBuilder();
            Property p = null;
            sbExpr.Append( "border-" );
            sbExpr.Append( propertyList.WmAbsToRel( PropertyList.Left ) );
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
                listprop = (ListProperty)propertyList.GetExplicitProperty( "border-left" );
                if ( listprop != null )
                {
                    // Get a parser for the shorthand to set the individual properties
                    IShorthandParser shparser = new GenericShorthandParser( listprop );
                    p = shparser.GetValueForProperty( PropName, this, propertyList );
                }
            }

            if ( p == null )
            {
                listprop = (ListProperty)propertyList.GetExplicitProperty( "border-width" );
                if ( listprop != null )
                {
                    // Get a parser for the shorthand to set the individual properties
                    IShorthandParser shparser = new BoxPropShorthandParser( listprop );
                    p = shparser.GetValueForProperty( PropName, this, propertyList );
                }
            }

            if ( p == null )
            {
                listprop = (ListProperty)propertyList.GetExplicitProperty( "border" );
                if ( listprop != null )
                {
                    // Get a parser for the shorthand to set the individual properties
                    IShorthandParser shparser = new GenericShorthandParser( listprop );
                    p = shparser.GetValueForProperty( PropName, this, propertyList );
                }
            }

            return p;
        }
    }
}