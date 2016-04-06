using System.Text;

namespace Fonet.Fo.Properties
{
    internal class EndIndentMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected EndIndentMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new EndIndentMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override bool IsCorrespondingForced( PropertyList propertyList )
        {
            var sbExpr = new StringBuilder();

            sbExpr.Length = 0;
            sbExpr.Append( "margin-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.End ) );

            if ( propertyList.GetExplicitProperty( sbExpr.ToString() ) != null )
                return true;

            return false;
        }


        public override Property Compute( PropertyList propertyList )
        {
            FObj parentFo = propertyList.GetParentFObj();
            var sbExpr = new StringBuilder();
            Property p = null;
            sbExpr.Append( "margin-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.End ) );

            if ( propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() ) == null )
                return p;
            sbExpr.Length = 0;

            sbExpr.Append( "_fop-property-value(" );
            sbExpr.Append( "margin-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.End ) );

            sbExpr.Append( ")" );
            sbExpr.Append( "+" );
            sbExpr.Append( "_fop-property-value(" );
            sbExpr.Append( "padding-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.End ) );

            sbExpr.Append( ")" );
            sbExpr.Append( "+" );
            sbExpr.Append( "_fop-property-value(" );
            sbExpr.Append( "border-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.End ) );
            sbExpr.Append( "-width" );
            sbExpr.Append( ")" );

            p = Make( propertyList, sbExpr.ToString(), propertyList.GetParentFObj() );

            if ( p != null )
                p = ConvertProperty( p, propertyList, parentFo );

            return p;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "0pt", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}