namespace Fonet.Fo.Properties
{
    internal class ContentHeightMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected ContentHeightMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ContentHeightMaker( propName );
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
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}