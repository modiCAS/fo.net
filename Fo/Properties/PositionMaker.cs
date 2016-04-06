namespace Fonet.Fo.Properties
{
    internal class PositionMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropStatic = new EnumProperty( Constants.Static );

        protected static readonly EnumProperty SPropRelative = new EnumProperty( Constants.Relative );

        protected static readonly EnumProperty SPropAbsolute = new EnumProperty( Constants.Absolute );

        protected static readonly EnumProperty SPropFixed = new EnumProperty( Constants.Fixed );

        private Property _mDefaultProp;

        protected PositionMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new PositionMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "static" ) )
                return SPropStatic;

            if ( value.Equals( "relative" ) )
                return SPropRelative;

            if ( value.Equals( "absolute" ) )
                return SPropAbsolute;

            if ( value.Equals( "fixed" ) )
                return SPropFixed;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "static", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}