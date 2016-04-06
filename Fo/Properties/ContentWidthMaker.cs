namespace Fonet.Fo.Properties
{
    internal class ContentWidthMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected ContentWidthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ContentWidthMaker( propName );
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