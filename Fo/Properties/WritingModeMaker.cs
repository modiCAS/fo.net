namespace Fonet.Fo.Properties
{
    internal class WritingModeMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropLrTb = new EnumProperty( Constants.LrTb );

        protected static readonly EnumProperty SPropRlTb = new EnumProperty( Constants.RlTb );

        protected static readonly EnumProperty SPropTbRl = new EnumProperty( Constants.TbRl );

        private Property _mDefaultProp;

        protected WritingModeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new WritingModeMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "lr-tb", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "lr-tb" ) )
                return SPropLrTb;

            if ( value.Equals( "rl-tb" ) )
                return SPropRlTb;

            if ( value.Equals( "tb-rl" ) )
                return SPropTbRl;

            return base.CheckEnumValues( value );
        }
    }
}