namespace Fonet.Fo.Properties
{
    internal class PageWidthMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected PageWidthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PageWidthMaker( propName );
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
                _mDefaultProp = Make( propertyList, "8in", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}