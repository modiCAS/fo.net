namespace Fonet.Fo.Properties
{
    internal class FontFamilyMaker : StringProperty.Maker
    {
        private Property m_defaultProp;

        protected FontFamilyMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FontFamilyMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "sans-serif", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}