namespace Fonet.Fo.Properties
{
    internal class TextAlignLastMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropCenter = new EnumProperty( Constants.Center );

        protected static readonly EnumProperty SPropEnd = new EnumProperty( Constants.End );

        protected static readonly EnumProperty SPropStart = new EnumProperty( Constants.Start );

        protected static readonly EnumProperty SPropJustify = new EnumProperty( Constants.Justify );

        private Property _mDefaultProp;

        protected TextAlignLastMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new TextAlignLastMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Compute( PropertyList propertyList )
        {
            Property computedProperty = null;
            Property correspondingProperty = propertyList.GetProperty( "text-align" );
            if ( correspondingProperty != null )
            {
                int correspondingValue = correspondingProperty.GetEnum();

                if ( correspondingValue == TextAlign.Justify )
                    computedProperty = new EnumProperty( Constants.Start );
                else if ( correspondingValue == TextAlign.End )
                    computedProperty = new EnumProperty( Constants.End );
                else if ( correspondingValue == TextAlign.Start )
                    computedProperty = new EnumProperty( Constants.Start );
                else if ( correspondingValue == TextAlign.Center )
                    computedProperty = new EnumProperty( Constants.Center );
            }
            return computedProperty;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "center" ) )
                return SPropCenter;

            if ( value.Equals( "end" ) )
                return SPropEnd;

            if ( value.Equals( "start" ) )
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