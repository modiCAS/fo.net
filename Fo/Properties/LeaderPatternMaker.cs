namespace Fonet.Fo.Properties
{
    internal class LeaderPatternMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropSpace = new EnumProperty( Constants.Space );

        protected static readonly EnumProperty SPropRule = new EnumProperty( Constants.Rule );

        protected static readonly EnumProperty SPropDots = new EnumProperty( Constants.Dots );

        protected static readonly EnumProperty SPropUsecontent = new EnumProperty( Constants.Usecontent );

        private Property _mDefaultProp;

        protected LeaderPatternMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new LeaderPatternMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "space" ) )
                return SPropSpace;

            if ( value.Equals( "rule" ) )
                return SPropRule;

            if ( value.Equals( "dots" ) )
                return SPropDots;

            if ( value.Equals( "use-content" ) )
                return SPropUsecontent;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "space", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}