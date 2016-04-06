namespace Fonet.Fo.Properties
{
    internal class ProvisionalDistanceBetweenStartsMaker : LengthProperty.Maker
    {
        private Property m_defaultProp;

        protected ProvisionalDistanceBetweenStartsMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ProvisionalDistanceBetweenStartsMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "24pt", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}