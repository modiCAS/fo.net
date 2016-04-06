namespace Fonet.Fo.Properties
{
    internal class ColumnGapMaker : LengthProperty.Maker
    {
        private Property m_defaultProp;

        protected ColumnGapMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ColumnGapMaker( propName );
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
                m_defaultProp = Make( propertyList, "0.25in", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}