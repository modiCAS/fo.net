namespace Fonet.Fo.Properties
{
    internal class FontStyleMaker : StringProperty.Maker
    {
        private Property _mDefaultProp;

        protected FontStyleMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FontStyleMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "normal", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}