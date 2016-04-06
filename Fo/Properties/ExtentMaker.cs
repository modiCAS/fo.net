namespace Fonet.Fo.Properties
{
    internal class ExtentMaker : LengthProperty.Maker
    {
        private Property m_defaultProp;

        protected ExtentMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ExtentMaker( propName );
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