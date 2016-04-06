namespace Fonet.Fo.Properties
{
    internal class NumberColumnsRepeatedMaker : NumberProperty.Maker
    {
        private Property _mDefaultProp;

        protected NumberColumnsRepeatedMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new NumberColumnsRepeatedMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "1", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}