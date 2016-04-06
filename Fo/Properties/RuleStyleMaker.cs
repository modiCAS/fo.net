namespace Fonet.Fo.Properties
{
    internal class RuleStyleMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropNone = new EnumProperty( Constants.None );

        protected static readonly EnumProperty SPropDotted = new EnumProperty( Constants.Dotted );

        protected static readonly EnumProperty SPropDashed = new EnumProperty( Constants.Dashed );

        protected static readonly EnumProperty SPropSolid = new EnumProperty( Constants.Solid );

        protected static readonly EnumProperty SPropDouble = new EnumProperty( Constants.Double );

        protected static readonly EnumProperty SPropGroove = new EnumProperty( Constants.Groove );

        protected static readonly EnumProperty SPropRidge = new EnumProperty( Constants.Ridge );

        private Property _mDefaultProp;

        protected RuleStyleMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new RuleStyleMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "none" ) )
                return SPropNone;

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

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "solid", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}