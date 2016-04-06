namespace Fonet.Fo.Properties
{
    internal class FontFamilyMaker : StringProperty.Maker
    {
        private Property _mDefaultProp;

        protected FontFamilyMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FontFamilyMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "sans-serif", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}