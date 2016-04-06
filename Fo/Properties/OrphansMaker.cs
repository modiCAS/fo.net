namespace Fonet.Fo.Properties
{
    internal class OrphansMaker : NumberProperty.Maker
    {
        private Property _mDefaultProp;

        protected OrphansMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new OrphansMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "2", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}