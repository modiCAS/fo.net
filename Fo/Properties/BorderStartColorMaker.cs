using System.Text;

namespace Fonet.Fo.Properties
{
    internal class BorderStartColorMaker : GenericColor
    {
        private Property _mDefaultProp;

        protected BorderStartColorMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderStartColorMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override bool IsCorrespondingForced( PropertyList propertyList )
        {
            var sbExpr = new StringBuilder();

            sbExpr.Length = 0;
            sbExpr.Append( "border-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.Start ) );
            sbExpr.Append( "-color" );
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
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.Start ) );
            sbExpr.Append( "-color" );
            p = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( p != null )
                p = ConvertProperty( p, propertyList, parentFo );

            return p;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "black", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}