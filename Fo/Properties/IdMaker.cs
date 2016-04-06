namespace Fonet.Fo.Properties
{
    internal class IdMaker : StringProperty.Maker
    {
        private Property _mDefaultProp;

        protected IdMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new IdMaker( propName );
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