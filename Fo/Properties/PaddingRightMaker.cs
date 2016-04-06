using System.Text;

namespace Fonet.Fo.Properties
{
    internal class PaddingRightMaker : GenericPadding
    {
        protected PaddingRightMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PaddingRightMaker( propName );
        }


        public override Property Compute( PropertyList propertyList )
        {
            FObj parentFo = propertyList.GetParentFObj();
            var sbExpr = new StringBuilder();
            Property p = null;
            sbExpr.Append( "padding-" );
            sbExpr.Append( propertyList.WmAbsToRel( PropertyList.Right ) );

            p = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( p != null )
                p = ConvertProperty( p, propertyList, parentFo );

            return p;
        }
    }
}