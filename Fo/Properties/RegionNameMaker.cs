namespace Fonet.Fo.Properties
{
    internal class RegionNameMaker : StringProperty.Maker
    {
        private Property _mDefaultProp;

        protected RegionNameMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new RegionNameMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}