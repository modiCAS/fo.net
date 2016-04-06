namespace Fonet.Fo.Properties
{
    internal class LetterValueMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty s_propALPHABETIC = new EnumProperty( Constants.ALPHABETIC );

        protected static readonly EnumProperty s_propTRADITIONAL = new EnumProperty( Constants.TRADITIONAL );

        protected static readonly EnumProperty s_propAUTO = new EnumProperty( Constants.AUTO );

        private Property m_defaultProp;

        protected LetterValueMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new LetterValueMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "alphabetic" ) )
                return s_propALPHABETIC;

            if ( value.Equals( "traditional" ) )
                return s_propTRADITIONAL;

            if ( value.Equals( "auto" ) )
                return s_propAUTO;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "auto", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}