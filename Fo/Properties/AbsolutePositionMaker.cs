namespace Fonet.Fo.Properties
{
    internal class AbsolutePositionMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropAuto = new EnumProperty( Constants.Auto );

        protected static readonly EnumProperty SPropFixed = new EnumProperty( Constants.Fixed );

        protected static readonly EnumProperty SPropAbsolute = new EnumProperty( Constants.Absolute );

        protected static readonly EnumProperty SPropInherit = new EnumProperty( Constants.Inherit );

        private Property _mDefaultProp;

        protected AbsolutePositionMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new AbsolutePositionMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "auto" ) )
                return SPropAuto;

            if ( value.Equals( "fixed" ) )
                return SPropFixed;

            if ( value.Equals( "absolute" ) )
                return SPropAbsolute;

            if ( value.Equals( "inherit" ) )
                return SPropInherit;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}