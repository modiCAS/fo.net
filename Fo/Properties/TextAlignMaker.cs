namespace Fonet.Fo.Properties
{
    internal class TextAlignMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropCenter = new EnumProperty( Constants.Center );

        protected static readonly EnumProperty SPropEnd = new EnumProperty( Constants.End );

        protected static readonly EnumProperty SPropStart = new EnumProperty( Constants.Start );

        protected static readonly EnumProperty SPropJustify = new EnumProperty( Constants.Justify );

        private Property _mDefaultProp;

        protected TextAlignMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new TextAlignMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "center" ) )
                return SPropCenter;

            if ( value.Equals( "end" ) )
                return SPropEnd;

            if ( value.Equals( "right" ) )
                return SPropEnd;

            if ( value.Equals( "start" ) )
                return SPropStart;

            if ( value.Equals( "left" ) )
                return SPropStart;

            if ( value.Equals( "justify" ) )
                return SPropJustify;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "start", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}