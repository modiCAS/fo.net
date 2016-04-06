namespace Fonet.Fo.Properties
{
    internal class PageHeightMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected PageHeightMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PageHeightMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        protected override bool IsAutoLengthAllowed()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "11in", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}