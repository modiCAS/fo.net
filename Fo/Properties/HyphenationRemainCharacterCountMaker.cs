namespace Fonet.Fo.Properties
{
    internal class HyphenationRemainCharacterCountMaker : NumberProperty.Maker
    {
        private Property _mDefaultProp;

        protected HyphenationRemainCharacterCountMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new HyphenationRemainCharacterCountMaker( propName );
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