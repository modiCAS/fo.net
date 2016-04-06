namespace Fonet.Fo.Properties
{
    internal class VerticalAlignMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropBaseline = new EnumProperty( Constants.Baseline );

        protected static readonly EnumProperty SPropMiddle = new EnumProperty( Constants.Middle );

        protected static readonly EnumProperty SPropSub = new EnumProperty( Constants.Sub );

        protected static readonly EnumProperty SPropSuper = new EnumProperty( Constants.Super );

        protected static readonly EnumProperty SPropTextTop = new EnumProperty( Constants.TextTop );

        protected static readonly EnumProperty SPropTextBottom = new EnumProperty( Constants.TextBottom );

        protected static readonly EnumProperty SPropTop = new EnumProperty( Constants.Top );

        protected static readonly EnumProperty SPropBottom = new EnumProperty( Constants.Bottom );

        private Property _mDefaultProp;

        protected VerticalAlignMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new VerticalAlignMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "baseline" ) )
                return SPropBaseline;

            if ( value.Equals( "middle" ) )
                return SPropMiddle;

            if ( value.Equals( "sub" ) )
                return SPropSub;

            if ( value.Equals( "super" ) )
                return SPropSuper;

            if ( value.Equals( "text-top" ) )
                return SPropTextTop;

            if ( value.Equals( "text-bottom" ) )
                return SPropTextBottom;

            if ( value.Equals( "top" ) )
                return SPropTop;

            if ( value.Equals( "bottom" ) )
                return SPropBottom;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "baseline", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}