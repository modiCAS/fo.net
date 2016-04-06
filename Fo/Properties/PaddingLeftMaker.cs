using System.Text;

namespace Fonet.Fo.Properties
{
    internal class PaddingLeftMaker : GenericPadding
    {
        protected PaddingLeftMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PaddingLeftMaker( propName );
        }


        public override Property Compute( PropertyList propertyList )
        {
            FObj parentFo = propertyList.GetParentFObj();
            var sbExpr = new StringBuilder();
            Property p = null;
            sbExpr.Append( "padding-" );
            sbExpr.Append( propertyList.WmAbsToRel( PropertyList.Left ) );

            p = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( p != null )
                p = ConvertProperty( p, propertyList, parentFo );

            return p;
        }
    }
}