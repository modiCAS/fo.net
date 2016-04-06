namespace Fonet.Fo.Properties
{
    internal class BottomMaker : LengthProperty.Maker
    {
        private Property m_defaultProp;

        protected BottomMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BottomMaker( propName );
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
                m_defaultProp = Make( propertyList, "auto", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}