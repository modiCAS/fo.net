namespace Fonet.Fo.Properties
{
    internal class LetterSpacingMaker : LengthProperty.Maker
    {
        private Property m_defaultProp;

        protected LetterSpacingMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new LetterSpacingMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "0pt", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}