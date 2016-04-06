namespace Fonet.Fo.Properties
{
    internal class InternalDestinationMaker : StringProperty.Maker
    {
        private Property _mDefaultProp;

        protected InternalDestinationMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new InternalDestinationMaker( propName );
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