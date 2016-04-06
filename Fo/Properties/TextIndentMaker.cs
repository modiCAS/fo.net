namespace Fonet.Fo.Properties
{
    internal class TextIndentMaker : LengthProperty.Maker
    {
        private Property m_defaultProp;

        protected TextIndentMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TextIndentMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "0pt", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}