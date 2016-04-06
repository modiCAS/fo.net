namespace Fonet.Fo.Properties
{
    internal class LeaderAlignmentMaker : EnumProperty.Maker
    {
        protected static readonly EnumProperty SPropNone = new EnumProperty( Constants.None );

        protected static readonly EnumProperty SPropReferenceArea = new EnumProperty( Constants.ReferenceArea );

        protected static readonly EnumProperty SPropPage = new EnumProperty( Constants.Page );

        private Property _mDefaultProp;

        protected LeaderAlignmentMaker( string name ) : base( name )
        {
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new LeaderAlignmentMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues( string value )
        {
            if ( value.Equals( "none" ) )
                return SPropNone;

            if ( value.Equals( "reference-area" ) )
                return SPropReferenceArea;

            if ( value.Equals( "page" ) )
                return SPropPage;

            return base.CheckEnumValues( value );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "none", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}