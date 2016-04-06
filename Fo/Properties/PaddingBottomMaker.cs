using System.Text;

namespace Fonet.Fo.Properties
{
    internal class PaddingBottomMaker : GenericPadding
    {
        protected PaddingBottomMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PaddingBottomMaker( propName );
        }


        public override Property Compute( PropertyList propertyList )
        {
            FObj parentFO = propertyList.getParentFObj();
            var sbExpr = new StringBuilder();
            Property p = null;
            sbExpr.Append( "padding-" );
            sbExpr.Append( propertyList.wmAbsToRel( PropertyList.BOTTOM ) );

            p = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( p != null )
                p = ConvertProperty( p, propertyList, parentFO );

            return p;
        }
    }
}