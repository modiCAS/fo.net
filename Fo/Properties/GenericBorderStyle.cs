namespace Fonet.Fo.Properties
{
    internal class GenericBorderStyle : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropNone = new EnumProperty( Enums.None );

        protected static readonly EnumProperty SPropHidden = new EnumProperty( Enums.Hidden );

        protected static readonly EnumProperty SPropDotted = new EnumProperty( Enums.Dotted );

        protected static readonly EnumProperty SPropDashed = new EnumProperty( Enums.Dashed );

        protected static readonly EnumProperty SPropSolid = new EnumProperty( Enums.Solid );

        protected static readonly EnumProperty SPropDouble = new EnumProperty( Enums.Double );

        protected static readonly EnumProperty SPropGroove = new EnumProperty( Enums.Groove );

        protected static readonly EnumProperty SPropRidge = new EnumProperty( Enums.Ridge );

        protected static readonly EnumProperty SPropInset = new EnumProperty( Enums.Inset );

        protected static readonly EnumProperty SPropOutset = new EnumProperty( Enums.Outset );

        private Property _mDefaultProp;

        protected GenericBorderStyle( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new GenericBorderStyle( propName );
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
                listprop = (ListProperty)propertyList.GetExplicitProperty( "border-style" );
                if ( listprop != null )
                {
                    IShorthandParser shparser = new BoxPropShorthandParser( listprop );
                    p = shparser.GetValueForProperty( PropName, this, propertyList );
                }
            }

            return p;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "none" ) )
                return SPropNone;

            if ( value.Equals( "hidden" ) )
                return SPropHidden;

            if ( value.Equals( "dotted" ) )
                return SPropDotted;

            if ( value.Equals( "dashed" ) )
                return SPropDashed;

            if ( value.Equals( "solid" ) )
                return SPropSolid;

            if ( value.Equals( "double" ) )
                return SPropDouble;

            if ( value.Equals( "groove" ) )
                return SPropGroove;

            if ( value.Equals( "ridge" ) )
                return SPropRidge;

            if ( value.Equals( "inset" ) )
                return SPropInset;

            if ( value.Equals( "outset" ) )
                return SPropOutset;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "none", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }

        internal class Enums
        {
            public const int None = Constants.None;

            public const int Hidden = Constants.Hidden;

            public const int Dotted = Constants.Dotted;

            public const int Dashed = Constants.Dashed;

            public const int Solid = Constants.Solid;

            public const int Double = Constants.Double;

            public const int Groove = Constants.Groove;

            public const int Ridge = Constants.Ridge;

            public const int Inset = Constants.Inset;

            public const int Outset = Constants.Outset;
        }
    }
}