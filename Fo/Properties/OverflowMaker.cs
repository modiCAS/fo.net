namespace Fonet.Fo.Properties
{
    internal class OverflowMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropVisible = new EnumProperty( Constants.Visible );

        protected static readonly EnumProperty SPropHidden = new EnumProperty( Constants.Hidden );

        protected static readonly EnumProperty SPropScroll = new EnumProperty( Constants.Scroll );

        protected static readonly EnumProperty SPropAuto = new EnumProperty( Constants.Auto );

        private Property _mDefaultProp;

        protected OverflowMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new OverflowMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "visible" ) )
                return SPropVisible;

            if ( value.Equals( "hidden" ) )
                return SPropHidden;

            if ( value.Equals( "scroll" ) )
                return SPropScroll;

            if ( value.Equals( "auto" ) )
                return SPropAuto;

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