using System.Text;

namespace Fonet.Fo.Properties
{
    internal class StartIndentMaker : LengthProperty.Maker
    {
        private Property m_defaultProp;

        protected StartIndentMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new StartIndentMaker( propName );
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
            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.START ) );

            if ( propertyList.GetExplicitProperty( sbExpr.ToString() ) != null )
                return true;

            return false;
        }


        public override Property Compute( PropertyList propertyList )
        {
            FObj parentFO = propertyList.getParentFObj();
            var sbExpr = new StringBuilder();
            Property p = null;
            sbExpr.Append( "margin-" );
            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.START ) );

            // Make sure the property is set before calculating it!
            if ( propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() ) == null )
                return p;
            sbExpr.Length = 0;

            sbExpr.Append( "_fop-property-value(" );
            sbExpr.Append( "margin-" );
            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.START ) );

            sbExpr.Append( ")" );
            sbExpr.Append( "+" );
            sbExpr.Append( "_fop-property-value(" );
            sbExpr.Append( "padding-" );
            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.START ) );

            sbExpr.Append( ")" );
            sbExpr.Append( "+" );
            sbExpr.Append( "_fop-property-value(" );
            sbExpr.Append( "border-" );
            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.START ) );
            sbExpr.Append( "-width" );
            sbExpr.Append( ")" );

            p = Make( propertyList, sbExpr.ToString(), propertyList.getParentFObj() );

            if ( p != null )
                p = ConvertProperty( p, propertyList, parentFO );

            return p;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "0pt", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}