using System.Text;

namespace Fonet.Fo.Properties
{
    internal class BorderStartColorMaker : GenericColor
    {
        private Property m_defaultProp;

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
            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.START ) );
            sbExpr.Append( "-color" );
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
            sbExpr.Append( "-color" );
            p = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( p != null )
                p = ConvertProperty( p, propertyList, parentFO );

            return p;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "black", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}