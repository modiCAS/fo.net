namespace Fonet.Fo.Properties
{
    internal class HyphenationCharacterMaker : CharacterProperty.Maker
    {
        private Property _mDefaultProp;

        protected HyphenationCharacterMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new HyphenationCharacterMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "-", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}