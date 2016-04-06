namespace Fonet.Fo.Properties
{
    internal class PageWidthMaker : LengthProperty.Maker
    {
        private Property m_defaultProp;

        protected PageWidthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PageWidthMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        protected override bool IsAutoLengthAllowed()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "8in", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}