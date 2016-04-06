namespace Fonet.Fo.Properties
{
    internal class BackgroundRepeatMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropRepeat = new EnumProperty( BackgroundRepeat.Repeat );

        protected static readonly EnumProperty SPropRepeatX = new EnumProperty( BackgroundRepeat.RepeatX );

        protected static readonly EnumProperty SPropRepeatY = new EnumProperty( BackgroundRepeat.RepeatY );

        protected static readonly EnumProperty SPropNoRepeat = new EnumProperty( BackgroundRepeat.NoRepeat );

        protected static readonly EnumProperty SPropInherit = new EnumProperty( BackgroundRepeat.Inherit );

        private Property _mDefaultProp;

        protected BackgroundRepeatMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BackgroundRepeatMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "repeat" ) )
                return SPropRepeat;

            if ( value.Equals( "repeat-x" ) )
                return SPropRepeatX;

            if ( value.Equals( "repeat-y" ) )
                return SPropRepeatY;

            if ( value.Equals( "no-repeat" ) )
                return SPropNoRepeat;

            if ( value.Equals( "inherit" ) )
                return SPropInherit;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "repeat", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}