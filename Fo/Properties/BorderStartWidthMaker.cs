using System.Text;

namespace Fonet.Fo.Properties
{
    internal class BorderStartWidthMaker : GenericCondBorderWidth
    {
        protected BorderStartWidthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderStartWidthMaker( propName );
        }


        public override bool IsCorrespondingForced( PropertyList propertyList )
        {
            var sbExpr = new StringBuilder();

            sbExpr.Length = 0;
            sbExpr.Append( "border-" );
            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.START ) );
            sbExpr.Append( "-width" );
            if ( propertyList.GetExplicitProperty( sbExpr.ToString() ) != null )
                return true;

            return false;
        }


        public override Property Compute( PropertyList propertyList )
        {
            FObj parentFO = propertyList.getParentFObj();
            var sbExpr = new StringBuilder();
            Property p = null;
            sbExpr.Append( "border-" );
            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.START ) );
            sbExpr.Append( "-width" );
            p = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( p != null )
                p = ConvertProperty( p, propertyList, parentFO );

            return p;
        }

        protected override string getDefaultForConditionality()
        {
            return "discard";
        }
    }
}