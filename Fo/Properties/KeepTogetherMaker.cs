namespace Fonet.Fo.Properties
{
    internal class KeepTogetherMaker : GenericKeep
    {
        private Property _mDefaultProp;

        protected KeepTogetherMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new KeepTogetherMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}