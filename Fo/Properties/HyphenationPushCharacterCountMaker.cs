namespace Fonet.Fo.Properties
{
    internal class HyphenationPushCharacterCountMaker : NumberProperty.Maker
    {
        private Property _mDefaultProp;

        protected HyphenationPushCharacterCountMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new HyphenationPushCharacterCountMaker( propName );
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