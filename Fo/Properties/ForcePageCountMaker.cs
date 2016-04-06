namespace Fonet.Fo.Properties
{
    internal class ForcePageCountMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropEven = new EnumProperty( Constants.Even );

        protected static readonly EnumProperty SPropOdd = new EnumProperty( Constants.Odd );

        protected static readonly EnumProperty SPropEndOnEven = new EnumProperty( Constants.EndOnEven );

        protected static readonly EnumProperty SPropEndOnOdd = new EnumProperty( Constants.EndOnOdd );

        protected static readonly EnumProperty SPropNoForce = new EnumProperty( Constants.NoForce );

        protected static readonly EnumProperty SPropAuto = new EnumProperty( Constants.Auto );

        private Property _mDefaultProp;

        protected ForcePageCountMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new ForcePageCountMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "even" ) )
                return SPropEven;

            if ( value.Equals( "odd" ) )
                return SPropOdd;

            if ( value.Equals( "end-on-even" ) )
                return SPropEndOnEven;

            if ( value.Equals( "end-on-odd" ) )
                return SPropEndOnOdd;

            if ( value.Equals( "no-force" ) )
                return SPropNoForce;

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