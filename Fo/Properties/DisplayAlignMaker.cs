namespace Fonet.Fo.Properties
{
    internal class DisplayAlignMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropBefore = new EnumProperty( Constants.Before );

        protected static readonly EnumProperty SPropAfter = new EnumProperty( Constants.After );

        protected static readonly EnumProperty SPropCenter = new EnumProperty( Constants.Center );

        protected static readonly EnumProperty SPropAuto = new EnumProperty( Constants.Auto );

        private Property _mDefaultProp;

        protected DisplayAlignMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new DisplayAlignMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "before" ) )
                return SPropBefore;

            if ( value.Equals( "after" ) )
                return SPropAfter;

            if ( value.Equals( "center" ) )
                return SPropCenter;

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