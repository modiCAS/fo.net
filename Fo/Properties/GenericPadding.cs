namespace Fonet.Fo.Properties
{
    internal class GenericPadding : LengthProperty.Maker
    {
        private Property m_defaultProp;

        protected GenericPadding( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new GenericPadding( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property GetShorthand( PropertyList propertyList )
        {
            Property p = null;
            ListProperty listprop;

            if ( p == null )
            {
                listprop = (ListProperty)propertyList.GetExplicitProperty( "padding" );
                if ( listprop != null )
                {
                    IShorthandParser shparser = new BoxPropShorthandParser( listprop );
                    p = shparser.GetValueForProperty( PropName, this, propertyList );
                }
            }

            return p;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "0pt", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}