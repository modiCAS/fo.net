namespace Fonet.Fo.Properties
{
    internal class RuleThicknessMaker : LengthProperty.Maker
    {
        private Property m_defaultProp;

        protected RuleThicknessMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new RuleThicknessMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "1.0pt", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}