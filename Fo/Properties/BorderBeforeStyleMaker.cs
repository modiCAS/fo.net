using System.Text;

namespace Fonet.Fo.Properties
{
    internal class BorderBeforeStyleMaker : GenericBorderStyle
    {
        protected BorderBeforeStyleMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderBeforeStyleMaker( propName );
        }


        public override bool IsCorrespondingForced( PropertyList propertyList )
        {
            var sbExpr = new StringBuilder();

            sbExpr.Length = 0;
            sbExpr.Append( "border-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.Before ) );
            sbExpr.Append( "-style" );
            if ( propertyList.GetExplicitProperty( sbExpr.ToString() ) != null )
                return true;

            return false;
        }


        public override Property Compute( PropertyList propertyList )
        {
            FObj parentFo = propertyList.GetParentFObj();
            var sbExpr = new StringBuilder();
            Property p = null;
            sbExpr.Append( "border-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.Before ) );
            sbExpr.Append( "-style" );
            p = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( p != null )
                p = ConvertProperty( p, propertyList, parentFo );

            return p;
        }
    }
}