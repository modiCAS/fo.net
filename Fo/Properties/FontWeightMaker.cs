namespace Fonet.Fo.Properties
{
    internal class FontWeightMaker : StringProperty.Maker
    {
        private Property _mDefaultProp;

        protected FontWeightMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FontWeightMaker( propName );
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