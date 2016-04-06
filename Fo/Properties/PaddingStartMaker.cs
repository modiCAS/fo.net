using System.Text;

namespace Fonet.Fo.Properties
{
    internal class PaddingStartMaker : GenericCondPadding
    {
        protected PaddingStartMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PaddingStartMaker( propName );
        }


        public override bool IsCorrespondingForced( PropertyList propertyList )
        {
            var sbExpr = new StringBuilder();

            sbExpr.Length = 0;
            sbExpr.Append( "padding-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.Start ) );

            if ( propertyList.GetExplicitProperty( sbExpr.ToString() ) != null )
                return true;

            return false;
        }


        public override Property Compute( PropertyList propertyList )
        {
            FObj parentFo = propertyList.GetParentFObj();
            var sbExpr = new StringBuilder();
            Property p = null;
            sbExpr.Append( "padding-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.Start ) );

            p = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( p != null )
                p = ConvertProperty( p, propertyList, parentFo );

            return p;
        }

        protected override string GetDefaultForConditionality()
        {
            return "discard";
        }
    }
}