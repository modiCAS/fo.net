namespace Fonet.Fo.Properties
{
    internal class TextDecorationMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropNone = new EnumProperty( Constants.None );

        protected static readonly EnumProperty SPropUnderline = new EnumProperty( Constants.Underline );

        protected static readonly EnumProperty SPropOverline = new EnumProperty( Constants.Overline );

        protected static readonly EnumProperty SPropLineThrough = new EnumProperty( Constants.LineThrough );

        protected static readonly EnumProperty SPropBlink = new EnumProperty( Constants.Blink );

        protected static readonly EnumProperty SPropNoUnderline = new EnumProperty( Constants.NoUnderline );

        protected static readonly EnumProperty SPropNoOverline = new EnumProperty( Constants.NoOverline );

        protected static readonly EnumProperty SPropNoLineThrough = new EnumProperty( Constants.NoLineThrough );

        protected static readonly EnumProperty SPropNoBlink = new EnumProperty( Constants.NoBlink );

        private Property _mDefaultProp;

        protected TextDecorationMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new TextDecorationMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "none" ) )
                return SPropNone;

            if ( value.Equals( "underline" ) )
                return SPropUnderline;

            if ( value.Equals( "overline" ) )
                return SPropOverline;

            if ( value.Equals( "line-through" ) )
                return SPropLineThrough;

            if ( value.Equals( "blink" ) )
                return SPropBlink;

            if ( value.Equals( "no-underline" ) )
                return SPropNoUnderline;

            if ( value.Equals( "no-overline" ) )
                return SPropNoOverline;

            if ( value.Equals( "no-line-through" ) )
                return SPropNoLineThrough;

            if ( value.Equals( "no-blink" ) )
                return SPropNoBlink;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "none", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}